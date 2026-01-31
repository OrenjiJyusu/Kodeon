package com.kylestudio.kodeon.task;

import com.kylestudio.kodeon.model.GameServer;
import com.kylestudio.kodeon.repository.GameServerRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;
import java.util.List;

@Component
public class ServerCleanupTask {

    @Autowired
    private GameServerRepository repository;

    // Run every 10 seconds
    @Scheduled(fixedRate = 10000)
    public void cleanupStaleServers() {
        LocalDateTime threshold = LocalDateTime.now().minusSeconds(30);
        
        List<GameServer> staleServers = repository.findAll().stream()
                .filter(s -> s.getLastHeartbeat() != null && s.getLastHeartbeat().isBefore(threshold))
                .toList();

        if (!staleServers.isEmpty()) {
            System.out.println("Cleaning up " + staleServers.size() + " stale servers...");
            repository.deleteAll(staleServers);
        }
    }
}
