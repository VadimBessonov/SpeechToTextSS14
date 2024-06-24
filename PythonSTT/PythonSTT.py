import speech_recognition as sr
import os
import sys

r = sr.Recognizer()
mic = sr.Microphone()


sr.LANGUAGE = 'ru-RU'

with mic as sourse:
    r.adjust_for_ambient_noise(sourse)
    audio = r.listen(sourse)

text = r.recognize_google(audio, language='ru-RU')
sys.stdout.write(text)






