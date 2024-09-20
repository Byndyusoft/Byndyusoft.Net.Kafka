docker compose up -d
docker exec kafka kafka-configs --bootstrap-server kafka:29092 --alter --add-config 'SCRAM-SHA-512=[password=broker]' --entity-type users --entity-name broker
docker exec kafka kafka-topics --bootstrap-server kafka:29092 --create --topic composer_assistant.entity.creation --partitions 9 --replication-factor 1