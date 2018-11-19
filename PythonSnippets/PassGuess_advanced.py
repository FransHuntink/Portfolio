# Some more advanced bruteforce
# -password guessing algorithms
# find me at franshuntink.com


import random
import itertools
import string
import threading
import multiprocessing 
import os
from itertools import permutations 
from itertools import product
from multiprocessing import Pool

 
# All variables used by the cracker 
result = ""
lowercase = string.ascii_lowercase
uppercase = string.ascii_uppercase
allchar = ''.join([chr(i) for i in range(33,127)])
numbers = string.digits

bodyTreshold = 4
bodyMinimum = 0
lengthLimit = 7
isFound = False
# calculate cartesian products outside of recursive method to
# -speed up the runtime of the method
password = "Dio2"

# when set to true, parameters will be increased based on an algorithm 
# -when failed to guess password with given info
def passwordSocial(UPPERCASE, BODY, NUMBERS):
	#print("UPPERCASE: " + str(UPPERCASE) +  " BODY:  " + str(BODY) + " NUMBERS: "+ str(NUMBERS))

	ucProduct = itertools.product(uppercase, repeat= UPPERCASE) # Cartesian product
	lcProduct = itertools.product(lowercase, repeat= BODY) # Cartesian product
	numProduct = itertools.product(numbers,  repeat= NUMBERS)  # Cartesian product

	varlist = list(lcProduct)
	var = len(varlist)

	varlist2 = list(numProduct)
	var2 = len(varlist2)

	global isFound
	global result
	result = "-"

	# Do the cartesian comparison
	for i in list(ucProduct): # uppercase iterator
		i = ''.join(i)
		for z in range(0,var): # lowercase iterator
			z = ''.join(varlist[z])
			if(i+z == password): 
					result = i + z
					isFound = True
					print("Found password: " + str(result))
					return 
			for x in range(0, var2): # number iterator
				x = ''.join(varlist2[x])
				if(i+z+x == password):
					result = i + z + x
					isFound = True
					print("Found password: " + str(result))
					return 
 
	# if the password body exceeds, we return 
	if(BODY >= bodyTreshold):
		return 
	# if the password was already guessed, we return
	if(isFound == False):
		passwordSocial(UPPERCASE, BODY+1, NUMBERS)
	else:
		return


#result = passwordSocial(0,0,0)
#print(result)

def QueueProcess(target, args):
		if __name__ == '__main__':
					pool = multiprocessing.Pool()
					results = [pool.apply_async(target, args= args[x]) for x in range(0,len(args))]
					output = [p.get() for p in results]
					

# The arguments that are run in paralell 
#args = [(1, 0, 0,), (0, 0, 0), (0, 0, 1), (1, 0, 1)]
args = [(1, 0, 1), (0,0,0)]
QueueProcess(passwordSocial, args)

# TO-DO
# Find out how to properly cancel other asyncs when password
# -is
