from tcp import tcp_server
from PIL import ImageShow
from console import prompt
from server import *
from json import load
from serialization import *
from os import getcwd

def position(camera,actual_position,target_position,base,lnk1,lnk2):

	print('Displaying camera.')
	ImageShow.show(camera)
	time = float(prompt('time ->'))
	base = float(prompt('base ->', '%.1f' % base))
	lnk1 = float(prompt('lnk1 ->', '%.1f' % lnk1))
	lnk2 = float(prompt('lnk2 ->', '%.1f' % lnk2))
	reset = False
	
	return time, base, lnk1, lnk2, reset

def manual_position():
    s = tcp_server()
    ip, port = get_server_settings()
    s.connect(ip, port)
    try:
        while True:
            packet				= s.receive_all	()
            a, b, c, d, e, f	= input_data	(packet)
            print("Actual position : ", b)
            a, b, c, d, e		= position	(a, b, c, d, e, f)
            packet				= output_data	(a, b, c, d, e)
            s.send_all(packet)
    except Exception as e:
        print(e)
        s.close()
