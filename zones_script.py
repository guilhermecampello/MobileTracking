import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

data = pd.read_csv("zones.csv",encoding="utf-8")
zones = data.groupby(['Position'])
legend=[]
i = 0

with open('zones_statistics.csv','w') as file:
  header = "Posição;Desvio Padrão;Média;Mediana;Mínimo;Máximo"
  file.write(header + "\n")
  for group in zones:
    statistics = "{0};{1};{2};{3};{4};{5}\n".format(group[0],group[1].MagY.std(),group[1].MagY.mean(),group[1].MagY.median(),group[1].MagY.min(), group[1].MagY.max())
    print(statistics)
    file.write(statistics)


  file.write("\n \n")
  file.write(header + "\n")
  for group in zones:
    statistics = "{0};{1};{2};{3};{4};{5}\n".format(group[0],group[1].MagZ.std(),group[1].MagZ.mean(),group[1].MagZ.median(),group[1].MagZ.min(), group[1].MagZ.max())
    print(statistics)
    file.write(statistics)

  file.write("\n \n")
  file.write(header + "\n")
  for group in zones:
    statistics = "{0};{1};{2};{3};{4};{5}\n".format(group[0],group[1].MagX.std(),group[1].MagX.mean(),group[1].MagX.median(),group[1].MagX.min(), group[1].MagX.max())
    print(statistics)
    file.write(statistics)

for group in zones:
  print(group[0])
  print(group[1])
  i+=1
  if i%2==0 or group[0] == "B0":
    color = (np.random.random(),np.random.random(),np.random.random())
    plt.scatter(group[1].MagY,group[1].MagZ,color=color)
    legend.append(group[0])

plt.title("Distribuição da intensidade do vetor campo magnético por posição")
plt.xlabel("Intensidade do campo magnético em Y [µT]")
plt.ylabel("Intensidade do campo magnético em Z [µT]")
plt.legend(legend)
plt.show()

for group in zones:
  print(group[0])
  print(group[1])
  i+=1
  if i%2==0 or group[0] == "B0":
    color = (np.random.random(),np.random.random(),np.random.random())
    plt.scatter(group[1].MagY.mean(),group[1].MagZ.mean(),color=color)
    plt.errorbar(group[1].MagY.mean(),group[1].MagZ.mean(),yerr=group[1].MagZ.std(),xerr=group[1].MagY.std() ,linestyle="None", color=color)

plt.title("Média da intensidade do vetor campo magnético por posição")
plt.xlabel("Intensidade do campo magnético em Y [µT]")
plt.ylabel("Intensidade do campo magnético em Z [µT]")
plt.legend(legend)
plt.show()

for group in zones:
  print(group[0])
  print(group[1])
  i+=1
  if i%2==0:
    color = (np.random.random(),np.random.random(),np.random.random())
    plt.scatter(group[1].Wifi,group[1].Bluetooth,color=color)

plt.title("Distribuição da intensidade do wifi e bluetooth por posição")
plt.xlabel("Intensidade do Wifi [dB]")
plt.ylabel("Intensidade do bluetooth [dB]")
plt.legend(legend)
plt.show()

for group in zones:
  print(group[0])
  print(group[1])
  i+=1
  if i%2==0:
    color = (np.random.random(),np.random.random(),np.random.random())
    plt.scatter(group[1].Wifi.median(),group[1].Bluetooth.median(),color=color)
    plt.errorbar(group[1].Wifi.median(),group[1].Bluetooth.median(),yerr=group[1].Bluetooth.std(),xerr=group[1].Wifi.std() ,linestyle="None", color=color)

plt.title("Mediana da intensidade dos sinais de Wifi e Bluetooth por posição")
plt.xlabel("Intensidade do Wifi [dB]")
plt.ylabel("Intensidade do bluetooth [dB]")
plt.legend(legend)
plt.show()

