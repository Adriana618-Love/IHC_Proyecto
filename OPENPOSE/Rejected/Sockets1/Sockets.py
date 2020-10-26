import sys
#Change the following line
#sys.path.append('C:\Users\Hasan\Downloads\opencv\sources\samples\python2')

import numpy as np
import cv2

import socket
import time
import sys
import base64

TCP_IP = ""
TCP_PORT = 1234

class SocketClass:
    def __init__(self):
        print("Target IP:", TCP_IP)
        print("Target port:", TCP_PORT)

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        print('Socket creado')
        
        try:
            self.sock.bind((TCP_IP, TCP_PORT))
            print('Socket bind complete')
        except socket.error as msg:
            print('Bind failed. Error Code : ' + str(msg[0]) + ' Message ' + msg[1])
            sys.exit()
            
        #Start listening on socket
        self.sock.listen(10)
        print ('Socket now listening')
        self.conn, self.addr = self.sock.accept()
        print ('Connected with ' + self.addr[0] + ':' + str(self.addr[1]) )
        #now keep talking with the client

    def __del__(self):
        self.sock.close()

    def receiveFrame(self):
        #wait to accept a connection - blocking call
        print ('top')
        
        #Receiving from client
        length=self.conn.recv(10)
        print ('length',length)
        length=int(length)
        print ('length',length)


        k=int(length/1024)
        j=length%1024

        maindata=bytes(b'')
        i=0
        while i<k:
            i=i+1
            data = self.conn.recv(1024)
            maindata=maindata+data
        if j!=0:    
            data = self.conn.recv(j)
            maindata=maindata+data
        #print('----------------------')
        #print(maindata)
        #print('----------------------')
        
        
        a= base64.b64decode(maindata) 
        
        nparr = np.fromstring(a, np.uint8)
        img_np = cv2.imdecode(nparr, cv2.IMREAD_COLOR )

        
        #mostrar frames recibidos
        print (img_np.shape)
        #cv2.imshow('frame',img_np)
        #cv2.waitKey(5)

        return img_np


    
    def sendMessage(self, message):
        bMessage = bytes(message, 'utf-8')
        #base64_bytes = base64.b64encode(bMessage)
        self.conn.sendall(bMessage)
    


"""
if __name__ == '__main__':
    import sys
    try: video_src = sys.argv[1]
    except: video_src = 0
    print (__doc__)
    SocketClass(video_src).run()
"""
