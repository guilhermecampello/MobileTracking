import matplotlib.pyplot as plt
import numpy as np

plt.show()

with open("campo_magnetico.csv") as arquivo:
  dados = arquivo.readlines()[1:]
  id = []
  x = []
  y = []
  z = []
  magX = []
  magY = []
  magZ = []
  colors = []
  for linha in dados:
    linha = linha.split(",")
    id.append(linha[0])
    x.append(float(linha[1]))
    y.append(float(linha[2]))
    z.append(0)
    magX.append(float(linha[3]))
    magY.append(float(linha[4]))
    magZ.append(float(linha[5]))
    colors.append(((float(linha[1])+1)/8,(float(linha[2])+1)/8, (float(linha[1])+float(linha[2])+1)/8))

  print(x)
  print(y)
  print(z)
  print(magX)
  print(magY)
  print(magZ)

ax = plt.figure().add_subplot(projection='3d')
ax.quiver(x, y, z, magX, magY, magZ, color=colors, length=0.1 ,arrow_length_ratio=0.1, alpha=1, normalize=True)

plt.title("Projeção 3D")
plt.show()

fig, ax = plt.subplots()
ax.quiver(x,y, magZ, magY, alpha = 1)
ax.set_aspect('equal')

# show plot
plt.title("Intensidades do vetor campo magnético Z(>) e Y(^) por coordenada")
plt.xlabel("Coordenada em X [m]")
plt.ylabel("Coordenada em Y [m]")
plt.show()

fig, ax = plt.subplots()
ax.quiver(x,y, magZ, magX, alpha = 1)
ax.set_aspect('equal')

# show plot
plt.title("Intensidades do vetor campo magnético Z(>) e X(^) por coordenada")
plt.xlabel("Coordenada em X [m]")
plt.ylabel("Coordenada em Y [m]")
plt.show()

fig, ax = plt.subplots()
ax.quiver(x,y, magX, magY, alpha = 1)
ax.set_aspect('equal')

# show plot
plt.title("Intensidades do vetor campo magnético X(>) e Y(^) por coordenada")
plt.xlabel("Coordenada em X [m]")
plt.ylabel("Coordenada em Y [m]")
plt.show()

