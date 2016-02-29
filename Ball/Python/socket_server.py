# start server with python3 -m http.server 10001
# install anaconda / python 3
# conda import matplotlib.pyplot
# conda import threading
# run with Python socket_server.py

import socket
import numpy as np
import matplotlib
matplotlib.use('TkAgg')
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import threading
import json
from collections import deque


samples = 100
data1 = deque(np.zeros(samples, dtype='f'), samples)
data2 = deque(np.zeros(samples, dtype='f'), samples)

global counter

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ('127.0.0.1', 10001)
sock.bind(server_address)
print('started up on %s port %s' % server_address)


def decode(d):
    return d.decode('utf-8')


def read_socket():
    global counter
    counter += 1

    print('\nwaiting to receive message')
    d, address = sock.recvfrom(4096)

    print('received %s bytes from %s' % (len(d), address))

    json_string = decode(d)
    # print(json_string)
    json_object = json.loads(json_string)
    print(json_object)
    if (counter % 2 == 0):
        data2.append(json_object['value'])
    else:
         data1.append(json_object['value'])
    	
    '''
    if d:
        sent = sock.sendto(d, address)
        print('sent %s bytes back to %s' % (sent, address))
    '''


def run_server():
    global counter
    counter = 0
    while True:
        read_socket()


thread = threading.Thread(target=run_server)
thread.daemon = True
thread.start()
print('listening')


fig = plt.figure()
ax = fig.add_subplot(1, 1, 1)
ax.set_xlim(0, samples)
ax.set_ylim(0, 3000)

line, = ax.plot([], [], 'b-')
line1, = ax.plot([], [], 'r-')

line.set_xdata(np.arange(samples))
line.set_ydata(np.zeros(samples))
line1.set_xdata(np.arange(samples))
line1.set_ydata(np.zeros(samples))


def init():
    line.set_visible(False)
    line1.set_visible(False)

    return [ax]


def update(frame_number):
    # if frame_number == 0:
    line.set_visible(True)
    line1.set_visible(True)

    line.set_ydata(data1)
    line1.set_ydata(data2)

    return [line, line1]


line_ani = animation.FuncAnimation(fig, update, init_func=init, interval=50, blit=True)

plt.show()
