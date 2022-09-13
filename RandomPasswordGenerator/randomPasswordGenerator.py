import string
import random
from random_word import RandomWords

specialChars = "! @ # $ % ^ & * ( ) 1 2 3 4 5 6 7 8 9 0"
r = RandomWords()

def randomPassword():
	listOfChars = specialChars.split(" ")
	random.shuffle(listOfChars)
	rWord = r.get_random_word(hasDictionaryDef="true", minLength=8, maxLength=12)
	password = []
	password.append(random.choice(listOfChars))
	password.append(rWord)
	password.append(random.choice(listOfChars))
	return("".join(password))

print(randomPassword())
