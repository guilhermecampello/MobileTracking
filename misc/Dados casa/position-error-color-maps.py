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

dados = pd.read_csv('PrecisionPerPosition-2-N3-USW1-_2021-11-24_16-11-41.csv')
print(dados)

# Convert from pandas dataframes to numpy arrays
X, Y, Z, = dados['X'], dados['Y'], dados['Error']

# create x-y points to be used in heatmap
xi = np.linspace(X.min(), X.max(), 1000)
yi = np.linspace(Y.min(), Y.max(), 1000)

# Interpolate for plotting
zi = griddata((X, Y), Z, (xi[None,:], yi[:,None]), method='linear')

# Create the contour plot
CS = plt.pcolormesh(xi, yi, zi)
for dado in dados.values:
    plt.scatter(dado[0],dado[1],10)
plt.colorbar()
plt.show()





