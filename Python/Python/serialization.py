from io		import BytesIO
from PIL	import Image
from struct import pack
from struct import unpack
from unity	import quaternion
from unity	import vector3

#
# serialization
#

# serialize value
def serialize(value, function = None):
	bytes = bytearray()
	if not function:
		if type(value) is bool:
			bytes = pack('?', value)
		elif type(value) is float:
			bytes = pack('f', value)
		elif type(value) is int:
			bytes = pack('i', value)
		elif type(value) is str:
			bytes = value.encode()
		elif type(value) is quaternion:
			bytes = pack('ffff', value.x, value.y, value.z, value.w)
		elif type(value) is vector3:
			bytes = pack('fff', value.x, value.y, value.z)
	else:
		bytes = function(value)

	size = pack('i', len(bytes))
	return size + bytes

# serialize all values
def serialize_all(*args):
	bytes = bytearray()
	for arg in args:
		bytes += serialize(arg)
	return bytes

#
# deserialization
#

# number of bytes in an integer
SIZEOF_INT = 4

# deserialize value
def deserialize(bytes, valueIndex, function):
	offset	= 0
	i		= 0
	while (i < valueIndex):
		size = unpack('i', bytes[offset : offset + SIZEOF_INT])[0]
		offset += SIZEOF_INT + size
		i += 1
	size = unpack('i', bytes[offset : offset + SIZEOF_INT])[0]
	offset += SIZEOF_INT
	return function(bytes[offset : offset + size])

# deserialize bool
def deserialize_bool(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : unpack('?', d)[0])

# deserialize camera
def deserialize_camera(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : Image.open(BytesIO(d)))

# deserialize float
def deserialize_float(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : unpack('f', d)[0])

# deserialize int
def deserialize_int(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : unpack('i', d)[0])

# deserialize string
def deserialize_string(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : d.decode())

# deserialize quaternion
def deserialize_quaternion(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : quaternion(unpack('f', d[:4])[0], unpack('f', d[4:8])[0], unpack('f', d[8:12])[0], unpack('f', d[12:16])[0]))

# deserialize vector3
def deserialize_vector3(data, valueIndex):
	return deserialize(data, valueIndex, lambda d : vector3(unpack('f', d[:4])[0], unpack('f', d[4:8])[0], unpack('f', d[8:12])[0]))
