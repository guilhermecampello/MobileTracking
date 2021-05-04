import numpy as np
import matplotlib.pyplot as plt

np.random.seed(19680801)

data = open("experiments.csv","r")
lines = data.readlines()
wifis = []
bluetooths = []
distances=[]

for line in lines:
  line_data = line.replace("\n","").split(",")
  dist = (float) (line_data[0])
  wifi = (int) (line_data[1])
  ble = (int) (line_data[2])
  if ble != 0:
    distances.append(dist)
    wifis.append(wifi)
    bluetooths.append(ble)

diff_distances = {}
count = 0
for d in distances:
  if d not in diff_distances:
    diff_distances[d] = 0
    count += 1

fig, ax = plt.subplots()
scatter = ax.scatter(wifis, bluetooths, c=distances)
plt.xlabel("Wifi [dB]")
plt.ylabel("Bluetooth [dB]")
legend1 = ax.legend(*scatter.legend_elements(num=None),  loc="upper left", title="Distâncias")
ax.add_artist(legend1)
plt.legend()
plt.show()

