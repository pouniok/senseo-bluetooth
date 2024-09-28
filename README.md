Senseo coffee machine controlled by Arduino and Android over bluetooth
Yes, this is really useless, so you should do it too.
This project is really easy-no-problem, TRUST ME, I would never lie to you. And ask a therapist friend to be here if you have one.
 
Origins
I don’t know how, but one day I found an article about the Arduino board. I had never heard of it, but when I saw it, I was like I NEED THIS I’M GONNA CHANGE THE WORLD. Then three months later, after I’ve made blink a led, I had no idea what to do with it…
But I think God with his capability to listen to every thought, caught the despair of my Arduino sleeping in the dust, and he broke the Senseo coffee machine of my office. Message received bro, I’ll take the Senseo and make it work even better than it was (you’re welcome).
So, the point of this, is to control this fucking machine over bluetooth, add an LCD screen so she can communicate with your stupid cow-orkers, and if we have the time, let it make coffee.
 
Disassembly (LOL EASY)
Get a hammer and smash it until it opens. Really, opening this machine is like the hardest part of the project, because when you are an engineer in the Senseo team, you spend all your time at the coffee machine, and with 95% caffeine in your blood, you do really stupid things. So, you have to remove the back of the machine, and you should see the water pump unless you are blind, and I don’t understand why the fuck you are reading this is you are fucking blind.

At the back of the water pump, there are two hidden screws at the bottom of two holes which are about 100 meters deep (we speak in metric here, it’s 2015 you know). Id you really want to open the machine without the hammer technique, you need a screwdriver in a “L” form with gears that let the screwdriver turn from outside the machine BECAUSE THEY LEFT NO SPACE FOR A NORMAL SCREWDRIVER TO FIT.
Once you have prayed enough to make the screws disappear, you can try to unclip the bottom part of the machine. It’s possible that your fingers brake before the clips, so be sure to have more than 4 fingers before going straight away in this project.
 
Retrieving information
I know how you feel right now, you are so happy it’s open that nothing could make you feel bad. I’m not saying that you’ll feel bad at the end of this part, but you will.
Now you have access to the green board, you can get information from the machine, so you have to solder some wires at the right place. BE CAREFUL IF YOU ARE A CHILD OR STUPID, there are some 230V parts in this circuit (told you, Senseo engineers…). If you have no electric knowledge like me, you can always wear your best suit while doing this, in case you die.
For the soldering part, I only had the wires you can plug into the breadboard, and it’s fucking bad because it’s taking a lot of place for nothing. When you solder the wires, try to not shake, or you’ll solder everything together. You should stop smoking about a year before starting this project.

You can pull 7 wires : ground, 1 cup, on/off, led, 2 cups, temperature, water level . If you ask yourself why there is only 6, look at the picture again and start looking for your intelligence.
Ok, now you have to close the the machine with the new wires. Do you remember I told you that you’d feel bad ? IT’LL NEVER CLOSE OR THE BUTTONS WILL BE PUSHED ALL THE TIME. I had to cut the fucking parts holding the board to let the wires have some more space, fucking wires, fucking machine, fucking engineers.
I added a DB25 socket (yes 25 pins for 7 wires, shut the fuck up I only had this one), but I would recommend to have a long wire out of the machine, it’s not sexy to have the socket connected on the machine.

Test your wires before closing, trust me you don’t want to open this thing twice.
 
Assembling
For the electronic part, I bought a wood box for 4$, and put everything in it.

Arduino UNO REV3
LCD 16×2
Bluetooth module HC-06
Relay module
Button switch
9V battery
Potentiometer
Breadboard
Wires
Just wire up everything together, be an artist, but remember to connect the ground of the Senseo on the ground of the Arduino. Or maybe it’s a joke and everything will burn. Or maybe it’s true.
Then, you should have something packed up in a box with an LCD, a switch, and something to connect to the Senseo.

 
Arduino code
The Arduino will control the Senseo, so you have to put some code in it. If you remember how the Senseo works, you’ll find that the buttons are not switches, you can only push them and they come up after activating something. So you have to send that kind of signal to the machine : HIGH then LOW for every action.
The water level is LOW if not enough water, and HIGH if there is enough, the temperature is boring and I won’t use it, but you can, on the analog pin, just find the value for when the water is hot enough, then you can write this value in a book and never use it again because IT’S USELESS.
The led will tell us everything we want. When the led is blinking the machine is not ready, and when the led is on, it is. And as you have the water level, and you can trace when the machine is working or not in the code, then you have everything you want.
The bluetooth is like  a Serial port, so you can just read what’s coming from Serial, and you’ll have the commands. I made the Arduino send the status of the Senseo every second, so my Android App always know the last information.

 
Controlling the Senseo from Android
Now, make an application that can connect to the bluetooth module, and make an interface to send the commands to the Arduino.

 
Just steal some icons and take 1 FUCKING HOUR TO REMOVE THE COPYRIGHT TEXTS ON IT, and you’ll be good to go.
 








Senseo V2 (twice bigger)
Now you can go back with the machine at your office and show everybody how miserable your life is. Spending time for adding bluetooth on a Senseo when you can’t put more than 2 coffee pods in it, really ?!

 
Pou

