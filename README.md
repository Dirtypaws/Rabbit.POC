#Prerequisite

* Ensure you've stopped and released all of the default containers for Rabbit
* `docker run -d --hostname rabbit --name dev -p 15682:15682 -p 15672:15672 -p 5672:5672 rabbitmq:3-management`

## How to Run

* Build the solution
* Open three console windows
* Change directory for two of them to Consumer Debug bin
* Change the third one to Publisher Debug Bin
* In the Publisher type ./Publisher -f 100

---

* Now change the line: https://github.com/Dirtypaws/Rabbit.POC/blob/master/Consumer/Program.cs#L41 to `True`. Repeat the steps above

### Observe how the behavior is different on the second run.