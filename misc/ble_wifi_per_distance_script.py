import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

data = pd.read_csv("ble_wifi_per_distance.csv")
data = data.replace({'Bluetooth': {0: np.nan }})
distances = data.groupby(['Distância'])
legend=[]
i = 0

for group in distances:
  fixedGroup = group[1].replace({'Bluetooth': {np.nan: group[1].Bluetooth.mean(skipna=True)}})
  color = (np.random.random(),np.random.random(),np.random.random())
  plt.scatter(fixedGroup.Wifi.mean(),fixedGroup.Bluetooth.mean(),color=color)
  plt.errorbar(fixedGroup.Wifi.mean(),fixedGroup.Bluetooth.mean(),yerr=fixedGroup.Bluetooth.std(),xerr=fixedGroup.Wifi.std(), linestyle="None", color=color)
  legend.append(group[0])

plt.title("Distribuição da intensidade dos sinais de Wifi e Bluetooth por distância")
plt.xlabel("RSSI Wifi [dB]")
plt.ylabel("RSSI Bluetooth [dB]")
plt.legend(legend)
plt.show()

for group in distances:
  fixedGroup = group[1].replace({'Bluetooth': {np.nan: group[1].Bluetooth.mean(skipna=True)}})
  color = (np.random.random(),np.random.random(),np.random.random())
  plt.scatter(group[0],fixedGroup.Wifi.mean(),color=(1,0,0))
  plt.scatter(group[0],fixedGroup.Bluetooth.mean(),color=(0,0,1))
  plt.errorbar(group[0],fixedGroup.Bluetooth.mean(),yerr=fixedGroup.Bluetooth.std(), linestyle="None", color=(0,0,1))
  plt.errorbar(group[0],fixedGroup.Wifi.mean(),yerr=fixedGroup.Wifi.std(), linestyle="None", color=(1,0,0))
  legend.append(group[0])

plt.title("Média da intensidade dos sinais de Wifi e Bluetooth por distância")
plt.ylabel("RSSI [dB]")
plt.xlabel("Distância [m]")
plt.show()

for group in distances:
  print(group[0])
  print(group[1])
  color = (np.random.random(),np.random.random(),np.random.random())
  plt.scatter(group[1].Wifi,group[1].Bluetooth,color=color)
  legend.append(group[0])

plt.legend(legend)
plt.show()

