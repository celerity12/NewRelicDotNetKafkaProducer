@echo off

echo.
echo === 1. Checking if Kafka is listening on port 9092 ===
netstat -an | findstr "9092" | findstr "LISTENING"
if %errorlevel% neq 0 (
    echo [ERROR] Kafka is NOT listening on port 9092. Please start Kafka before continuing.
    pause
    exit /b
) else (
    echo [SUCCESS] Kafka is running and listening on port 9092.
)
echo.
pause

echo === 2. Listing all topics on localhost:9092 ===
CALL c:\kafka\bin\windows\kafka-topics.bat --bootstrap-server localhost:9092 --list
if %errorlevel% neq 0 (
    echo [ERROR] Failed to list Kafka topics.
    pause
    exit /b
)
echo.
echo Above are the topics currently available in your Kafka server.
pause

echo === 3. Creating topic 'demo-topic' (if it does not exist) ===
echo [INFO] This step creates a topic named 'demo-topic'.
echo [INFO] An 'Topic already exists' message is not a failure.
CALL c:\kafka\bin\windows\kafka-topics.bat --bootstrap-server localhost:9092 --create --topic demo-topic --partitions 1 --replication-factor 1
if %errorlevel% neq 0 (
    echo [WARNING] The command returned an error, which is expected if the topic already exists.
) else (
    echo [SUCCESS] Topic 'demo-topic' was created successfully.
)
echo.
pause

echo === 4. PRODUCING A MESSAGE TO 'demo-topic' ===
echo You now need to open a NEW Command Prompt window for the producer.
echo In the new window, run:
echo     c:\kafka\bin\windows\kafka-console-producer.bat --broker-list localhost:9092 --topic demo-topic
echo Type your message and press Enter to send it to the topic.
echo Press Ctrl+C in that window to stop the producer when done.
echo.
pause

echo === 5. CONSUMING MESSAGES FROM 'demo-topic' ===
echo You now need to open ANOTHER NEW Command Prompt window for the consumer.
echo In the new window, run:
echo     c:\kafka\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic demo-topic --from-beginning
echo You will see all messages sent to 'demo-topic', including those you produced.
echo Press Ctrl+C in that window to stop the consumer when done.
echo.
pause

echo === All steps completed. You can now produce and consume messages using the instructions above. ===
pause