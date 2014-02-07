import csv
from matplotlib.pyplot import *

def union (dict1, dict2):
    return dict(dict1.items() + dict2.items())

def read_csv (filename, coltypes, colnames=None):
    colnames = colnames or {}
    colnames = union({col:col for col in coltypes}, colnames)

    def convert (typ, val):
        try: return typ(val)
        except: return None

    def read_row (raw):
        return {colnames[col]: convert(typ, raw[col]) for col, typ in coltypes.items()}
    return [read_row(row) for row in csv.DictReader(open(filename))]

def col (data, colname):
    return [row[colname] for row in data]


####################
###   Gestures   ###
####################

lastYs = [0,0,0,0,0]
increasing = True
def simple (skel):
    global lastYs, increasing
    for i in range(len(lastYs)-1):
        lastYs[i] = lastYs[i+1]
    lastYs[-1] = skel["Y"]
    if increasing:
        if lastYs[-1] < lastYs[-2] < lastYs[-3]:
            increasing = False
            return 1
    else:
        if lastYs[-1] > lastYs[-2] > ys[-3]:
            increasing = True
            return 2


################
###   Main   ###
################

d = read_csv("JoeSlow.csv", {"Time":float, "HandRight.Y":float}, {"HandRight.Y":"Y"})

ts = col(d, "Time")
ys = col(d, "Y")
beats = [row["Time"] for row in d if simple(row)]

plot(ts, ys, 'ro')
vlines(beats, min(y for y in ys if y != None), max(ys))
show()