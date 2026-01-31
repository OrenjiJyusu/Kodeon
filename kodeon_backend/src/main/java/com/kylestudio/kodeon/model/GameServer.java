package com.kylestudio.kodeon.model;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.AllArgsConstructor;

import java.time.LocalDateTime;

@Entity
@Table(name = "game_servers")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class GameServer {

    @Id
    private String serverId; // Unique ID (IP:Port or UUID)
    
    private String name; // Server Name (host player name)
    
    @com.fasterxml.jackson.annotation.JsonProperty("isDedicated")
    private boolean isDedicated; // True = Headless, False = Player Host
    
    private String ipAddress;
    private int port;
    private int playerCount;
    private int maxPlayers;
    
    private LocalDateTime lastHeartbeat;
    private String status; // ONLINE, IN_MATCH, OFFLINE
}
