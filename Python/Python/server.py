from serialization import *
from os import getcwd
from platform_utils	import on_windows
from platform_utils	import on_linux
from json import load

# format data received from unity
def input_data (packet):
	return (deserialize_camera	(packet, 0), 
			deserialize_vector3	(packet, 5), 
			deserialize_vector3	(packet, 1), 
			deserialize_float	(packet, 2), 
			deserialize_float	(packet, 3), 
			deserialize_float	(packet, 4))

# format data to send to unity
def output_data(time, base, lnk1, lnk2, reset):
	assert type(time) is float, 'time is not a float'
	assert type(base) is float, 'base is not a float'
	assert type(lnk1) is float, 'lnk1 is not a float'
	assert type(lnk2) is float, 'lnk2 is not a float'
	assert type(reset) is bool, 'reset is not a bool'

	return serialize_all(
		time, 
		base, 
		lnk1, 
		lnk2, 
		None,
		reset
	)

# get server settings
def get_server_settings():
	cwd = getcwd()
	if on_linux():
		path = cwd[0 : len(cwd) - len(r'/Python/Python')] + r'/server.json'
	elif on_windows:
		path = cwd[0: len(cwd)-len(r'\Python\Python')] + r'\server.json'
	print("\n" + path)
	file = open(path, 'r')
	settings = load(file)
	print(settings)
	return settings['ip'], settings['port']

#Reception
def from_server(server): 
    data = input_data(server.receive_all())
    #print("[FROM SERVER] actual_position = %s, target_position = %s, base = %.2f, lnk1 = %.2f, lnk2 = %.2f" % (str(data[1]), str(data[2]), data[3],data[4],data[5]))
    return data

#Sending
def to_server(server, data):
    #print("[TO SERVER] time = %.2f, base = %.2f, lnk1 = %.2f, lnk2 = %.2f, reset = %r" % (data[0],data[1],data[2],data[3],data[4]))
    server.send_all(output_data(*data))
