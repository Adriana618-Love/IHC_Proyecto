import sys
import socket
import time
import sys
import base64
from threading import Thread 
import random
import time

TCP_IP = "127.0.0.1"
TCP_PORT = 2000
TCP_IP2 = "26.162.26.142"

probabilidad_spikes = 0.3
sleep_cometas = 4
sleep_ballonSpikes = 8

globo_conectado = True
ventilador_conectado = True

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
            #print ("Server recibio datos:", data)

            if(data == b'AC'):
                print ("Globo envio AC")
                globo_conectado = True
                continue
            elif(data == b'QQ'):
                print ("Cerrando conexion con globo")
                break
            #========transmitir mensaje recibido
            try:
                self.conn_ventilador.sendall(data)
                print ("Server envio datos:", data)
            except socket.error as msg:
                #print('Ventilador se desconecto')

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
            #print ("Server recibio datos:", data)

            if(data == b'AC'):
                print ("Ventilador envio AC")
                globo_conectado = True
                continue
            elif(data == b'QQ'):
                print ("Cerrando conexion con ventilador")
                break
            #========transmitir mensaje recibido
            try:
                self.conn_globo.sendall(data)
                print ("Server envio datos:", data)
            except socket.error as msg:
                #print('Ventilador se desconecto')
            
class Thread_verificarConexion(Thread): 
    def __init__(self, globo_conectado_, ventilador_conectado_): 
        Thread.__init__(self) 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Generador cometas corriendo.' )

    def run(self): 
        try:
            while (True):
                data = self.generateCometas()
                data = bytes(data, 'utf-8')
                self.conn_globo.sendall(data)
                self.conn_ventilador.sendall(data)
                print ("Server envio a ambos datos [cometa]:", data)    
                # Sleep for a TIME
                time.sleep(sleep_cometas)
        except socket.error as msg:
            print('Sockets cerrados - thread Cometa')

    def generateCometas(self):
        mensaje = "C"

        #seleccionar tipo de cometa
        rand = random.randint(0, 1)
        mensaje += str(rand)

        #seleccionar la posicion en X
        rand = str(random.randint(0, 36))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        return mensaje


class Thread_generadorSPikes(Thread): 
    def __init__(self, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Generador spikes corriendo.' )

    def run(self): 
        try:
            while (True):
                time.sleep(1)
                data = self.generateSpikes()
                data = bytes(data, 'utf-8')
                self.conn_ventilador.sendall(data)
                self.conn_globo.sendall(data)
                print ("Server envio a ambos datos [spikes]:", data)    
                # Sleep for a TIME
                time.sleep(30)
        except socket.error as msg:
            print('Sockets cerrados - thread Spikes')

    def generateSpikes(self):
        mensaje = "S"
        for i in range(25):
            # 0 - trampolin, 1 - spikes
            rand = random.uniform(0, 1)

            if(rand > probabilidad_spikes):
                mensaje += "0"
            else:
                mensaje += "1"
        return mensaje

class Thread_generadorCometas(Thread): 
    def __init__(self, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Generador cometas corriendo.' )

    def run(self): 
        try:
            while (True):
                data = self.generateCometas()
                data = bytes(data, 'utf-8')
                self.conn_globo.sendall(data)
                self.conn_ventilador.sendall(data)
                print ("Server envio a ambos datos [cometa]:", data)    
                # Sleep for a TIME
                time.sleep(sleep_cometas)
        except socket.error as msg:
            print('Sockets cerrados - thread Cometa')

    def generateCometas(self):
        mensaje = "C"

        #seleccionar tipo de cometa
        rand = random.randint(0, 1)
        mensaje += str(rand)

        #seleccionar la posicion en X
        rand = str(random.randint(0, 36))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        return mensaje

class Thread_generadorBallonSpikes(Thread): 
    def __init__(self, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Generador balloon spikes corriendo.' )

    def run(self): 
        try:
            while (True):
                time.sleep(1)
                data = self.generateBalloonSpikes()
                data = bytes(data, 'utf-8')
                self.conn_globo.sendall(data)
                self.conn_ventilador.sendall(data)
                print ("Server envio a ambos datos [balloon spikes]:", data)    
                # Sleep for a TIME
                time.sleep(sleep_ballonSpikes)
        except socket.error as msg:
            print('Sockets cerrados - thread Ballon Spikes')

    def generateBalloonSpikes(self):
        mensaje = "B"

        #seleccionar la posicion en X
        rand = str(random.randint(0, 36))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        #seleccionar la posicion en Y
        rand = str(random.randint(0, 18))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        return mensaje


class Thread_generadorBallonSpikes(Thread): 
    def __init__(self, conn_globo, conn_ventilador): 
        Thread.__init__(self) 
        self.conn_globo = conn_globo 
        self.conn_ventilador = conn_ventilador 
        print ('Generador balloon spikes corriendo.' )

    def run(self): 
        try:
            while (True):
                time.sleep(1)
                data = self.generateBalloonSpikes()
                data = bytes(data, 'utf-8')
                self.conn_globo.sendall(data)
                self.conn_ventilador.sendall(data)
                print ("Server envio a ambos datos [balloon spikes]:", data)    
                # Sleep for a TIME
                time.sleep(sleep_ballonSpikes)
        except socket.error as msg:
            print('Sockets cerrados - thread Ballon Spikes')

    def generateBalloonSpikes(self):
        mensaje = "B"

        #seleccionar la posicion en X
        rand = str(random.randint(0, 36))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        #seleccionar la posicion en Y
        rand = str(random.randint(0, 18))
        while(len(rand) < 2):
            rand = '0'+ rand
        mensaje += str(rand)

        return mensaje


def verificarConexionGlobo(conn_ventilador, globo_conectado_):
    while True:
        if(globo_conectado_):
            globo_conectado_ = False
        else:
            print("Globo Desconecto")
            data = bytes('GD', 'utf-8')
            conn_ventilador.sendall(data)
        time.sleep(4)

def verificarConexionVentilador(conn_globo, ventilador_conectado_):
    while True:
        if(ventilador_conectado_):
            ventilador_conectado_ = False
        else:
            print("Ventilador Desconecto")
            data = bytes('VD', 'utf-8')
            conn_globo.sendall(data)
        time.sleep(4)

class SocketClass:
    def __init__(self):
        print("Target IP:", TCP_IP)
        print("Target port:", TCP_PORT)

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) 
        print('Socket creado')

        self.sock2 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock2.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) 
        
        try:
            self.sock.bind((TCP_IP, TCP_PORT))
            self.sock2.bind((TCP_IP2, TCP_PORT))
            print('Socket bind complete')
        except socket.error as msg:
            print('Bind failed. Error Code : ' + str(msg[0]) + ' Message ' + msg[1])
            sys.exit()
            
        #leyendo cliente globo
        self.sock.listen(4) 
        print ("Esperando cliente globo...") 
        (conn_globo, (ip_globo,port_globo)) = self.sock.accept()
        ##Enviando mensaje al globo
        self.enviarMensaje(conn_globo, "AG")
        print ("Globo aceptado...") 
        time.sleep(1)
        
        # #leyendo cliente ventilador
        self.sock2.listen(4) 
        print ("Esperando cliente ventilador...") 
        (conn_ventilador, (ip_vent,port_vent)) = self.sock2.accept()
        ##Enviando mensaje al ventilador
        self.enviarMensaje(conn_ventilador, "AV")
        print ("Ventilador aceptado...")

        # #Esperando 5 segundos
        time.sleep(5)
        
        #Enviando mensaje de inicio a ambas conexiones
        self.enviarMensaje(conn_globo, "EE")
        self.enviarMensaje(conn_ventilador, "EE")
        
        #Esperar 1 segundo
        time.sleep(1)
         
        
        print ("Creando threads...")  

        globo_thread = ClientThread_globo(ip_globo,port_globo,conn_globo,conn_ventilador) 
        globo_thread.start() 
        vent_thread = ClientThread_ventilador(ip_vent,port_vent,conn_globo,conn_ventilador) 
        vent_thread.start() 

        #correr threads que detectan acks
        Thread(target=verificarConexionGlobo, args=(conn_ventilador, globo_conectado))
        Thread(target=verificarConexionVentilador, args=(conn_globo, ventilador_conectado))

        #cometas
        cometas_thread = Thread_generadorCometas(conn_globo,conn_ventilador) 
        cometas_thread.start() 

        #ballon spikes
        ballonSpikes_thread = Thread_generadorBallonSpikes(conn_globo,conn_ventilador) 
        ballonSpikes_thread.start() 

        #spikes
        spikes_thread = Thread_generadorSPikes(conn_globo,conn_ventilador) 
        spikes_thread.start()

    def enviarMensaje(self, conn, data):
        data = bytes(data, 'utf-8')
        print("Resultado",conn.sendall(data))

if __name__ == '__main__':
    print("aa")
    socketClass = SocketClass()

