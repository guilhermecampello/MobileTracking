from numpy.core.fromnumeric import size
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from scipy.interpolate import griddata

def normalize(vector):
    result = vector.copy()
    max_value = vector.max()
    min_value = vector.min()
    result = (vector - min_value) / (max_value - min_value)
    return result

dados = pd.read_csv('dados_ble.csv')
print(dados)
pot_mW = 10**(dados['Strength']/10)
normalizado = normalize(pot_mW)
print(normalizado)


# Convert from pandas dataframes to numpy arrays
X, Y, Z, = dados['X'], dados['Y'], dados['Strength']

# create x-y points to be used in heatmap
xi = np.linspace(X.min(), X.max(), 1000)
yi = np.linspace(Y.min(), Y.max(), 1000)

# Interpolate for plotting
zi = griddata((X, Y), Z, (xi[None,:], yi[:,None]), method='cubic')
csv = ''
for linha in zi:
    for i in linha:
        csv+= str(i)+','
    csv += '\n'

grid_csv = open('wifi_grid.csv', 'w')
grid_csv.write(csv)


# Create the contour plot
CS = plt.pcolormesh(xi, yi, zi)
for dado in dados.values:
    plt.scatter(dado[1],dado[2],5)
plt.colorbar()
plt.show()





