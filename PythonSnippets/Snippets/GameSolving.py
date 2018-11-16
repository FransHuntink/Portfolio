# Some code snippets by Frans Huntink
# find me at franshuntink.com

import random
import itertools
import string
import threading
import multiprocessing
from itertools import permutations 
from itertools import product



# FizBuzz implementation 
def fizbuzz():
	for i in range(1,101):
		if(i%3 == 0 and i%5 == 0):
			print("FizzBuzz")
		elif (i%3 == 0):
			print("Fizz")
		elif(i%5 == 0):
			print("Buzz")
		else:
			print(i)
#fizzbuzz()


# Recursive FizzBuzz implementation 
def fizzbuzzRecur(index):
	if(index > 100):
		print("End")
	else:
		if(index%3 == 0 and index%5 == 0):
			print("FizzBuzz")
		elif (index%3 == 0):
			print("Fizz")
		elif (index%5 == 0):
			print("Buzz")
		else:
			print(index)
		fizzbuzzRecur(index+1)
#fizzbuzzRecur(0)



# Solves sevenGame/Skip with given table, max of (table * 10) + 1
def sevenGame(table):
	for i in range(1,(table*10) + 1):
		if(i%table == 0):
			print("Skip!")
		elif( (i-table ) % 10 == 0):
			print("Skip!")
		else:
			print(i)
#sevenGame(7)
			

# Solves sevenGame/Skip with given table using parsing #
def sevenGameNext(table, factor):
	for i in range(1,table * factor + 1):
		parsed = str(i)
		if(i%table ==0):
			print("Skip!")
		elif(str(table) in parsed):
			print("Skip!")
		else:
			print(i)
#sevenGameNext(7, 10)


