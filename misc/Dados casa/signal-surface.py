import matplotlib.pyplot as plt
from matplotlib import cm
from matplotlib.ticker import LinearLocator
import numpy as np
from numpy.core.fromnumeric import size
import pandas as pd
from scipy.interpolate import griddata

def normalize(vector):
    result = vector.copy()
    max_value = vector.max()
    min_value = vector.min()
    result = (vector - min_value) / (max_value - min_value)
    return result

dados = pd.read_csv('ble_max_strength.csv')
print(dados)
pot_mW = 10**(dados['max']/10)
normalizado = normalize(pot_mW)
print(normalizado)


# Convert from pandas dataframes to numpy arrays
X, Y, Z, = dados['X'], dados['Y'], 100+dados['max']
print(X)
print(Y)
print(Z)
# create x-y points to be used in heatmap
xi = np.linspace(X.min(), X.max(), 1000)
yi = np.linspace(Y.min(), Y.max(), 1000)

# Interpolate for plotting
zi = griddata((X, Y), Z, (xi[None,:], yi[:,None]), method='cubic', fill_value=0)
xi,yi = np.meshgrid(xi,yi)


fig = plt.figure(num=1, clear=True)
ax = fig.add_subplot(1, 1, 1, projection='3d')

ax.plot_surface(xi, yi, zi, cmap=cm.coolwarm)
ax.set(xlabel='x', ylabel='y', zlabel='z', title='Bluetooth')

plt.show()

