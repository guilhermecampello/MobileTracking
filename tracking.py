from flask import Flask
from flask import request
import json

app = Flask(__name__)
file = open("calibrations.txt","a+")

@app.route('/origin', methods=['POST'])
def calibrations_collection():
  global file
  if request.method == 'POST':
    file.write(request.data.decode('ascii') + "\n")
    return "Ok", 200
