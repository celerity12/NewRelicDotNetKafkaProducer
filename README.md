# HellowWebAppSR

#LicenseKey

#UserKey


#Download DotNet Agent
https://download.newrelic.com/dot_net_agent/latest_release/NewRelicDotNetAgent_x64.msi

#By default, the New Relic .NET agent stores its log files on Windows in the following directory:

C:\ProgramData\New Relic\.NET Agent\Logs


#List of Urls
http://localhost:8082/Home/Index
http://localhost:8082/Home/Privacy
http://localhost:8082/Home/GetWeather


#API
dotnet add package NewRelic.Agent.Api --version 10.41.0


#Starting Kafka
# Zookeeper .\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
# Server/Broker .\bin\windows\kafka-server-start.bat .\config\server.properties
# Create Topic
# .\bin\windows\kafka-topics.bat --create --topic test --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
# .\bin\windows\kafka-topics.bat --create --topic my-first-topic --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
# List Topics
# .\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092
# Produce Messages Separate Terminal
# .\bin\windows\kafka-console-producer.bat --topic my-first-topic --bootstrap-server localhost:9092
# .\bin\windows\kafka-console-producer.bat --topic test --bootstrap-server localhost:9092
# .\bin\windows\kafka-console-producer.bat --topic my-first-topic --bootstrap-server localhost:9092
# consume Messages Separate Terminal
# .\bin\windows\kafka-console-consumer.bat --topic my-first-topic --from-beginning --bootstrap-server localhost:9092
# .\bin\windows\kafka-console-consumer.bat --topic test --from-beginning --bootstrap-server localhost:9092#   N e w R e l i c D o t N e t K a f k a P r o d u c e r  
 