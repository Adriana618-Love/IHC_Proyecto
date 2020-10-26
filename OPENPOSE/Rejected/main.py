import argparse

import cv2
import numpy as np
import torch

from models.with_mobilenet import PoseEstimationWithMobileNet
from modules.keypoints import extract_keypoints, group_keypoints
from modules.load_state import load_state
from modules.pose import Pose, track_poses
from val import normalize, pad_width
from Sockets import SocketClass

class ImageReader(object):
    def __init__(self, file_names):
        self.file_names = file_names
        self.max_idx = len(file_names)

    def __iter__(self):
        self.idx = 0
        return self

    def __next__(self):
        if self.idx == self.max_idx:
            raise StopIteration
        img = cv2.imread(self.file_names[self.idx], cv2.IMREAD_COLOR)
        if img.size == 0:
            raise IOError('Image {} cannot be read'.format(self.file_names[self.idx]))
        self.idx = self.idx + 1
        return img


class VideoReader(object):
    def __init__(self, file_name):
        self.file_name = file_name
        try:  # OpenCV needs int to read from webcam
            self.file_name = int(file_name)
        except ValueError:
            pass

    def __iter__(self):
        self.cap = cv2.VideoCapture(self.file_name)
        if not self.cap.isOpened():
            raise IOError('Video {} cannot be opened'.format(self.file_name))
        return self

    def __next__(self):
        was_read, img = self.cap.read()
        if not was_read:
            raise StopIteration
        return img


def infer_fast(net, img, net_input_height_size, stride, upsample_ratio, cpu,
               pad_value=(0, 0, 0), img_mean=(128, 128, 128), img_scale=1/256):
    height, width, _ = img.shape
    scale = net_input_height_size / height

    scaled_img = cv2.resize(img, (0, 0), fx=scale, fy=scale, interpolation=cv2.INTER_CUBIC)
    scaled_img = normalize(scaled_img, img_mean, img_scale)
    min_dims = [net_input_height_size, max(scaled_img.shape[1], net_input_height_size)]
    padded_img, pad = pad_width(scaled_img, stride, pad_value, min_dims)

    tensor_img = torch.from_numpy(padded_img).permute(2, 0, 1).unsqueeze(0).float()
    if not cpu:
        tensor_img = tensor_img.cuda()

    stages_output = net(tensor_img)

    stage2_heatmaps = stages_output[-2]
    heatmaps = np.transpose(stage2_heatmaps.squeeze().cpu().data.numpy(), (1, 2, 0))
    heatmaps = cv2.resize(heatmaps, (0, 0), fx=upsample_ratio, fy=upsample_ratio, interpolation=cv2.INTER_CUBIC)

    stage2_pafs = stages_output[-1]
    pafs = np.transpose(stage2_pafs.squeeze().cpu().data.numpy(), (1, 2, 0))
    pafs = cv2.resize(pafs, (0, 0), fx=upsample_ratio, fy=upsample_ratio, interpolation=cv2.INTER_CUBIC)

    return heatmaps, pafs, scale, pad


def run_detector(net, img, height_size, cpu, track, smooth, socketClass):
    net = net.eval()
    if not cpu:
        net = net.cuda()

    stride = 8
    upsample_ratio = 4
    num_keypoints = Pose.num_kpts
    previous_poses = []
    delay = 33


    #orig_img = img.copy()
    heatmaps, pafs, scale, pad = infer_fast(net, img, height_size, stride, upsample_ratio, cpu)

    total_keypoints_num = 0
    all_keypoints_by_type = []
    for kpt_idx in range(num_keypoints):  # 19th for bg
        total_keypoints_num += extract_keypoints(heatmaps[:, :, kpt_idx], all_keypoints_by_type, total_keypoints_num)

    pose_entries, all_keypoints = group_keypoints(all_keypoints_by_type, pafs, demo=True)
    for kpt_id in range(all_keypoints.shape[0]):
        all_keypoints[kpt_id, 0] = (all_keypoints[kpt_id, 0] * stride / upsample_ratio - pad[1]) / scale
        all_keypoints[kpt_id, 1] = (all_keypoints[kpt_id, 1] * stride / upsample_ratio - pad[0]) / scale
    current_poses = []

    for n in range(len(pose_entries)):
        if len(pose_entries[n]) == 0:
            continue
        pose_keypoints = np.ones((num_keypoints, 2), dtype=np.int32) * -1
        for kpt_id in range(num_keypoints):
            if pose_entries[n][kpt_id] != -1.0:  # keypoint was found
                pose_keypoints[kpt_id, 0] = int(all_keypoints[int(pose_entries[n][kpt_id]), 0])
                pose_keypoints[kpt_id, 1] = int(all_keypoints[int(pose_entries[n][kpt_id]), 1])
        pose = Pose(pose_keypoints, pose_entries[n][18])
        
        moves, numMoves = detectMove(pose_keypoints)
        message = messageProtocol(pose_keypoints)
        print('----------------------')
        print('moves: ' + str(moves) + ', numMoves: '+ str(numMoves))
        print('message: ')
        message=str(numMoves) + moves + message
        print(message)
        socketClass.sendMessage(message)

        current_poses.append(pose)
    """
    if track:
        track_poses(previous_poses, current_poses, smooth=smooth)
        previous_poses = current_poses
    for pose in current_poses:
        pose.draw(img)

    img = cv2.addWeighted(orig_img, 0.6, img, 0.4, 0)

    cv2.imshow('Lightweight Human Pose Estimation Python Demo', img)
    """
    key = cv2.waitKey(delay)
    if key == 27:  # esc
        return
    elif key == 112:  # 'p'
        if delay == 33:
            delay = 0
        else:
            delay = 33
  
def detectMove(keypoints):
    nariz = keypoints[0]
    cuello = keypoints[1]
    hombroI = keypoints[2]
    manoI = keypoints[4]
    hombroD = keypoints[5]
    manoD = keypoints[7]
    caderaI = keypoints[8]
    pieI = keypoints[10]
    caderaD = keypoints[11]
    pieD = keypoints[13]

    if(hombroD[0] != -1 and hombroI[0] != -1):
        distPies = (hombroD[0] - hombroI[0])/2
    else:
        distPies = 50
    
    """
    print("----------------pose_keypoints--------------")
    print("distancia entre los pies: " + str(distPies))
    print("nariz: " + str(nariz) + ", cuello"  + str(cuello) )
    print("hombroI: " + str(hombroI) + ", manoI"  + str(manoI) )
    print("hombroD: " + str(hombroD) + ", manoD"  + str(manoD) )
    print("caderaI: " + str(caderaI) + ", pieI"  + str(pieI) )
    print("caderaD: " + str(caderaD) + ", pieD"  + str(pieD) )
    """
    moves = ''
    numMoves = 0
    #if((nariz[0] < cuello[0]-5) or (nariz[0] > cuello[0]+5)):
    #    moves += "CB"
    #    numMoves += 1
    if((manoI[1] != -1) and (hombroI[1] != -1) and (manoI[1] <= hombroI[1])):
        moves += "BD"
        numMoves += 1
    if((manoD[1] != -1) and (hombroD[1] != -1) and (manoD[1] <= hombroD[1])):
        moves += "BI"
        numMoves += 1
    if((pieI[0] != -1) and (caderaI[0] != -1) and (pieI[0] < (caderaI[0]-distPies))):
        moves += "PD"
        numMoves += 1
    if((pieD[0] != -1) and (caderaD[0] != -1) and (pieD[0] > (caderaD[0]+distPies))):
        moves += "PI"
        numMoves += 1
    print(moves)
    return moves, numMoves

def resizeToThree(message):
    message = str(message)
    while(len(message) < 3):
        message = '0' + message
        #print("--" + message)
    return message

def messageProtocol(pose_keypoints):
    points = ""
    for i in range(len(pose_keypoints)):
        points += resizeToThree(pose_keypoints[i][0]) + resizeToThree(pose_keypoints[i][1])
    return points

if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        description='''Lightweight human pose estimation python demo.
                       This is just for quick results preview.
                       Please, consider c++ demo for the best performance.''')
    parser.add_argument('--checkpoint-path', type=str, required=True, help='path to the checkpoint')
    parser.add_argument('--height-size', type=int, default=256, help='network input layer height size')
    parser.add_argument('--video', type=str, default='', help='path to video file or camera id')
    parser.add_argument('--images', nargs='+', default='', help='path to input image(s)')
    parser.add_argument('--cpu', action='store_true', help='run network inference on cpu')
    parser.add_argument('--track', type=int, default=1, help='track pose id in video')
    parser.add_argument('--smooth', type=int, default=1, help='smooth pose keypoints')
    args = parser.parse_args()

    if args.video == '' and args.images == '':
        raise ValueError('Either --video or --image has to be provided')

    net = PoseEstimationWithMobileNet()
    checkpoint = torch.load(args.checkpoint_path, map_location='cpu')
    load_state(net, checkpoint)

    cap = cv2.VideoCapture(0)
    while(True):
        socketClass = SocketClass()
        while(not socketClass.receiveQuit()):
            #frame = socketClass.receiveFrame()
            ret, frame = cap.read()
            run_detector(net, frame, args.height_size, args.cpu, args.track, args.smooth, socketClass)