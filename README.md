# IHC_Proyecto

Todo el proyecto aquí realizado es compatible con Unity, no hay mayor dificultad que añadir todo el proyecto a Unity.
 * Unity Versión 2020.1.3f1
 * Windows 10

## Para ejecutar el servidor
### Requisitos:
 * CUDA 10.1
 * Python +3.6
 * OpenCV
 * Pycocotools
 * Pytorch
Tranquilo, todo esto esta dentro de la carpeta OPENPOSE, en el archivo requirements.txt y el comando para ejecutar el servidor se encuentra en RunCodeComand.txt o te lo dejamos aquí:
* `python demo.py --checkpoint-path <path_to>/checkpoint_iter_370000.pth --video 0`

Para correr el modelo, puedes descargarlo pre-entrenado de: https://download.01.org/opencv/openvino_training_extensions/models/human_pose_estimation/checkpoint_iter_370000.pth.

Se recomienda un espacio moderadamente amplio para poder jugar, asi como una laptop con cámara web.
