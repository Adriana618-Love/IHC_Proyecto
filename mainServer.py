import sys
import numpy as np
import cv2
import socket
import time
import sys
import base64
from threading import Thread 

TCP_IP = "26.223.87.228"
TCP_PORT = 2000

class ClientThread_globo(Thread): 

    def __init__(self,ip,port, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.ip = ip 
        self.port = port 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Conectado con ' + ip + ':' + str(port) )

    def run(self): 
        while True : 
            data = self.conn_globo.recv(2) 
            print ("Server recibio datos:", data)

            if(data == b'QQ'):
                print ("Cerrando conexion con globo")
                break
            #========transmitir mensaje recibido
            self.conn_ventilador.sendall(data)
            print ("Server envio datos:", data)

class ClientThread_ventilador(Thread): 
    def __init__(self,ip,port, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.ip = ip 
        self.port = port 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Conectado con ' + ip + ':' + str(port) )

    def run(self): 
        while True : 
            data = self.conn_ventilador.recv(2) 
            print ("Server recibio datos:", data)

            if(data == b'QQ'):
                print ("Cerrando conexion con ventilador")
                break
            #========transmitir mensaje recibido
            self.conn_globo.sendall(data)
            print ("Server envio datos:", data)



class SocketClass:
    def __init__(self):
        print("Target IP:", TCP_IP)
        print("Target port:", TCP_PORT)

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) 
        print('Socket creado')
        
        try:
            self.sock.bind((TCP_IP, TCP_PORT))
            print('Socket bind complete')
        except socket.error as msg:
            print('Bind failed. Error Code : ' + str(msg[0]) + ' Message ' + msg[1])
            sys.exit()
            
        #leyendo cliente globo
        self.sock.listen(4) 
        print ("Esperando cliente globo...") 
        (conn_globo, (ip_globo,port_globo)) = self.sock.accept() 
        print ("Globo aceptado...") 
        
        #leyendo cliente ventilador
        self.sock.listen(4) 
        print ("Esperando cliente ventilador...") 
        (conn_ventilador, (ip_vent,port_vent)) = self.sock.accept() 
        print ("Ventilador aceptado...") 
        
        print ("Creando threads...") 
        globo_thread = ClientThread_globo(ip_globo,port_globo,conn_globo,conn_ventilador) 
        globo_thread.start() 
        vent_thread = ClientThread_ventilador(ip_vent,port_vent,conn_globo,conn_ventilador) 
        vent_thread.start() 

    """
    def receiveQuit(self):
        try:
            quit = self.conn.recv(1)
            #print(quit)
            if(quit == b'Q'):
                print ('Mensaje de salida recibido', quit)
                #self.sendMessage('OK')
                self.sock.close()
                return True
            return False
        except:
            self.sock.close()
            return True

    
    def sendMessage(self, message):
        bMessage = bytes(message, 'utf-8')
        #base64_bytes = base64.b64encode(bMessage)
        self.conn.sendall(bMessage)
    """



if __name__ == '__main__':
    print("aa")
    socketClass = SocketClass()

