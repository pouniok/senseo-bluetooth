#include <LiquidCrystal.h>

/**
 * Code C pour Arduino permettant de convertir la cafetière Senseo en bluetooth
 */

// variables
int BTN_ONOFF = 8;
int BTN_1CUP = 9;
int BTN_2CUP = 10;
int LED = 11;
int TEMP = 1;
int WATER = 13;

LiquidCrystal lcd(2, 3, 4, 5, 6, 7);

void setup() {
  pinMode(BTN_ONOFF, OUTPUT);
  pinMode(BTN_1CUP, OUTPUT);
  pinMode(BTN_2CUP, OUTPUT);
  pinMode(LED, INPUT);
  pinMode(TEMP, INPUT);
  pinMode(WATER, INPUT);

  digitalWrite(BTN_ONOFF, HIGH);
  digitalWrite(BTN_1CUP, HIGH);
  digitalWrite(BTN_2CUP, HIGH);
  
  lcd.begin(16, 2);
  initLCD("Ready");
  
  Serial.begin(9600);
}

void loop() {
  
  // wait for character to arrive
   while (Serial.available() == 0); 
   char c = Serial.read();
   
   // Temperature
   int temp = analogRead(TEMP);
   int water = digitalRead(WATER);
   
   switch (c) {
     case 'f':
       initLCD("Power OFF");
       digitalWrite(BTN_ONOFF, LOW);
       delay(500);
       digitalWrite(BTN_ONOFF, HIGH);
       Serial.write("OK");
     break;
     
     case 'o':
       initLCD("Power ON");
       digitalWrite(BTN_ONOFF, LOW);
       delay(500);
       digitalWrite(BTN_ONOFF, HIGH);
       Serial.write("OK");
     break;
     
     case '1':
       initLCD("1 Tasse");
       digitalWrite(BTN_1CUP, LOW);
       delay(500);
       digitalWrite(BTN_1CUP, HIGH);
     break;
     
     case '2':
       initLCD("2 Tasses");
       digitalWrite(BTN_2CUP, LOW);
       delay(500);
       digitalWrite(BTN_2CUP, HIGH);
     break;
     
     case 't':
       lcd.clear();
       lcd.print("Temperature");
       lcd.setCursor(0, 1);
       lcd.print(10000.0 / (temp*1.0), 0);
       lcd.print(" C");
       delay(5000);
       initLCD("Ready");
     break;
      
     case 'w':
       if (water == 0) {
         initLCD("Remettre eau");
         Serial.write("KO");
       }
       else {
         initLCD("Niveau eau OK");
         Serial.write("OK");
       }
     break;
     
     default:
       initLCD("Ready");
     break;
   }
 
   
   delay(500);
}

void initLCD(String message) {
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Senseo");
  lcd.setCursor(0, 1);
  lcd.print(message);
}
