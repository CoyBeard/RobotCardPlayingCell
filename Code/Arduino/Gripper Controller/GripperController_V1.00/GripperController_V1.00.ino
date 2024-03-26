
const int TriggerInpPin = 2;
const int IO1Pin = 4;
const int IO2Pin = 6;
const int PumpRelayPin = 8;
const int ValveRelayPin = 10;
const int LEDPin = 12;

int MinOnTime = 200; //ms

void setup() {
  //Serial
  Serial.begin(9600);

  //Pin Setup
  pinMode(TriggerInpPin, INPUT_PULLUP);//_PULLUP
  pinMode(PumpRelayPin, OUTPUT);
  pinMode(ValveRelayPin, OUTPUT);
  pinMode(LEDPin, OUTPUT);
/*
  //Startup Sequence
  digitalWrite(LEDPin, HIGH);
  delay(200);
  digitalWrite(LEDPin, LOW);
  delay(100);
  digitalWrite(LEDPin, HIGH);
  delay(200);
  digitalWrite(LEDPin, LOW);
  delay(100);
  digitalWrite(LEDPin, HIGH);
  delay(200);
  digitalWrite(LEDPin, LOW);delay(100);
  */

}

void loop() {

  Serial.print(digitalRead(TriggerInpPin));
  Serial.print("\n");
  //If Trigger Is Activated
  if (digitalRead(TriggerInpPin) == 0) {
    

    digitalWrite(PumpRelayPin, HIGH);
    digitalWrite(LEDPin, HIGH);
    delay(MinOnTime);

    while (digitalRead(TriggerInpPin) == 0){
      Serial.print(digitalRead(TriggerInpPin));
      Serial.print("\n");

      digitalWrite(PumpRelayPin, HIGH);
      digitalWrite(LEDPin, HIGH);
    } 

    if (digitalRead(TriggerInpPin) == 1){
      digitalWrite(ValveRelayPin, HIGH);
      //delay(100);

      digitalWrite(PumpRelayPin, LOW);
      delay(1000);

      digitalWrite(ValveRelayPin, LOW);
      digitalWrite(LEDPin, LOW);

      //delay(1000);
    } 
  }
  

}
