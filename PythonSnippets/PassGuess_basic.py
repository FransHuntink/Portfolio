# Some code snippets by Frans Huntink 
# find me at franshuntink.com


import random
import itertools
import string
import threading
import multiprocessing
from itertools import permutations 
from itertools import product


# number chatsetthe guesser will use
charSetNum = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

# ascii charset that ranges from 0-95 
charSet = ''.join([chr(i) for i in range(33,127)])




# Attempts to guess a password on a totally random basis, obviously not efficient #
def passwordRandom(password):
	print('Randomly cracking password')
	maxAttemps = 1000000
	curAttempts = 0
	passList = list()
	passList = [None] * len(password)
	result = ""
	for i in range(0,maxAttemps):
		for x in range(0, len(password)):
			num = charSetNum[random.randint(0,9)]
			passList[x] = num
		curAttempts += 1
		print(passList)
		if(passList == password):
			print("Found password after attempts: " + str(curAttempts) +", password = " +  str(passList))
			return
		else:
			result = " "

randPass = [1, 2, 3, 4, 5, 6]
#passwordRandom(randPass)


# Attempts to crack password by bruteforce permutations 
def passwordCombination(password, charset):
	print('Cracking password using cartesian product')
	cProduct = itertools.product(charSet, repeat= len(password)) # Cartesian product
	attempts = 0

	for i in cProduct:
		attempts += 1
		i = ''.join(i)
		#print(i)
		if(i == password):
			print('Found password after attempts : ' + str(attempts) + ', password : ' + str(i))
			return
	else:
		print("Could not crack password")

combiPass = 'cc'
combiPass2 = 'dd'
combiPass3 = 'ee'
combiPass4 = 'ff'
#passwordCombination(combiPass, charSet)
#passwordCombination(combiPass2, charSet)


# Threading example of how these crackers could be run in paralell
runThreading = False
if(runThreading):
	thread1 = threading.Thread(target = passwordCombination, args = (combiPass, charSet,))
	thread2 = threading.Thread(target = passwordCombination, args = (combiPass2, charSet,))
	thread1.start()
	thread2.start()
	thread1.join()
	thread2.join()


# MultiProcessing example of how these crackers could be run in paralell 
# Until CPU limit is reached, time of each password should stay similar 
# -because more memory and CPU space are opened up to calculate each pass

runMultiproc = True
if __name__ == '__main__':
	if(runMultiproc):
		multiProc1 = multiprocessing.Process(target = passwordCombination, args = (combiPass, charSet,))
		multiProc2 = multiprocessing.Process(target = passwordCombination, args = (combiPass2, charSet,))
		multiProc3 = multiprocessing.Process(target = passwordCombination, args = (combiPass3, charSet,))
		multiProc4 = multiprocessing.Process(target = passwordCombination, args = (combiPass4, charSet,))
	
		multiProc1.start()
		multiProc2.start()
		multiProc3.start()
		multiProc4.start()

		multiProc1.join()
		multiProc2.join()
		multiProc3.join()
		multiProc4.join()
