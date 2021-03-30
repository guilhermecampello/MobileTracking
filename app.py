from flask import Flask
from flask import request
import json

app = Flask(__name__)

@app.route('/origin', methods=['POST'])
def calibrations_collection():
  if request.method == 'POST':
    with open("calibrations.csv","a+") as file:
      file.write(request.data.decode('ascii') + "\n")
      file.close()
    return "Ok", 200

@app.route('/experiment', methods=['POST'])
def experiments_collection():
  if request.method == 'POST':
    with open("experiments.csv","a+") as file:
      file.write(request.data.decode('ascii') + "\n")
      file.close()
    return "Ok", 200
