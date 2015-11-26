from django.shortcuts import render
from django.http import HttpResponse
from django.shortcuts import render
import json
import urllib.request
import datetime

def users(request):
    response = urllib.request.urlopen("http://localhost:20524/getallusers").read()
    userdata = json.loads(response.decode())
    context = {'userdata' : userdata}
    return render(request, 'dashboard.html', context)