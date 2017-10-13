from socket import socket
from socket import AF_INET
from socket import SOCK_STREAM

from struct import unpack
from struct import pack

import sys

# tcp server
class tcp_server:

	# wait for client to connect
	def connect(self, ip, port):
		print('\nInitializing server...')
		s = socket(AF_INET, SOCK_STREAM)
		try:
			s.bind((ip, port))
		except Exception as e:
			print('Error: Invalid ip address. Have you configured the server.json file in the root folder?\n')
			s.close()
			sys.exit()
		s.listen(1)
		print('Server initialized.')
		print('Waiting for client to connect...')
		self.client, _ = s.accept()
		print('Client connected.')

	# send entire packet
	def send_all(self, packet):
		size = pack('i', len(packet))
		self.client.sendall(size + packet)

	# receive entire packet
	def receive_all(self, bufferSize = 1024):
		packet = bytearray()
		packetSize = 0
		while True:
			fragment = self.client.recv(bufferSize)
			if packetSize == 0:
				packetSize = unpack('i', fragment[:4])[0]
				packet += fragment[4 : len(fragment)]
			else:
				packet += fragment
			if (len(packet) == packetSize):
				break
		return packet

	# close connection
	def close(self):
		print('Closing server...')
		self.client.close()
		print('Server closed.')