import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from scipy.fft import fft, fftfreq
from os import listdir
legends = []
testes = ["QG4.csv","BD.csv"]

for f in listdir("MagneticTimelines"):
  print(f)
  if f in testes:
    df = pd.read_csv("MagneticTimelines/"+f)
    startIndex = 0
    endIndex = 300
    samples = endIndex-startIndex
    df = df[startIndex:endIndex]
    duration = (df.Time.values[-1] - df.Time.values[0])
    print("Duração:{0}".format(duration))
    sample_rate = 1/(duration/samples)
    print("Frequência:{0}".format(sample_rate))

    #plt.plot( 'Time', 'X', data=df, color='skyblue', linewidth=1)
    #plt.plot( 'Time', 'Y', data=df, marker='', color='red', linewidth=1)
    #plt.plot( 'Time', 'Z', data=df, marker='', color='green', linewidth=1)
    # show legend

    yf = fft(df.Norm.values - df.Norm.mean())
    xf= fftfreq(samples,duration/samples)

    plt.plot(xf,-20*np.log10(np.abs(yf/samples)))
    legends.append(f.split(".")[0])

plt.legend(legends)
plt.title("FFT - Campo magnético")
plt.show()

