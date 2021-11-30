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

dados = pd.read_csv('PrecisionPerPosition-2-N3-USW0,33-_2021-11-29_13-10-14.csv')
print(dados)

# Convert from pandas dataframes to numpy arrays
X, Y, Z, = dados['X'], dados['Y'], dados['Error']

# create x-y points to be used in heatmap
xi = np.linspace(X.min(), X.max(), 1000)
yi = np.linspace(Y.min(), Y.max(), 1000)

# Interpolate for plotting
zi = griddata((X, Y), Z, (xi[None,:], yi[:,None]), method='cubic')

# Create the contour plot

plt.pcolormesh(xi, yi, zi, cmap='RdYlGn_r')
for dado in dados.values:
    plt.scatter(dado[0],dado[1],10)

plt.title('Erro médio por coordenada normalizado')
plt.set_cmap('RdYlGn_r')
plt.xlabel('X [m]')
plt.ylabel('Y [m]')
plt.colorbar()
plt.show()





