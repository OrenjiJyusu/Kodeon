package com.kylestudio.kodeon.repository;

import com.kylestudio.kodeon.model.GameServer;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Repository
public interface GameServerRepository extends JpaRepository<GameServer, String> {
    List<GameServer> findByStatus(String status);
    
    // Find servers that missed heartbeat (e.g., older than 1 minute)
    List<GameServer> findByLastHeartbeatBefore(LocalDateTime cutoff);
}
