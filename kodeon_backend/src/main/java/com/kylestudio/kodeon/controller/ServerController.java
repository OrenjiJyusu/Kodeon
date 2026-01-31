package com.kylestudio.kodeon.controller;

import com.kylestudio.kodeon.model.GameServer;
import com.kylestudio.kodeon.repository.GameServerRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

@RestController
@RequestMapping("/server")
public class ServerController {

    @Autowired
    private GameServerRepository serverRepository;

    @PostMapping("/register")
    public ResponseEntity<GameServer> registerServer(@RequestBody GameServer server) {
        // Simple registration logic
        if (server.getServerId() == null || server.getServerId().isEmpty()) {
            server.setServerId(UUID.randomUUID().toString());
        }
        server.setLastHeartbeat(LocalDateTime.now());
        server.setStatus("ONLINE");
        return ResponseEntity.ok(serverRepository.save(server));
    }

    @PostMapping("/heartbeat/{id}")
    public ResponseEntity<Void> heartbeat(@PathVariable String id) {
        return serverRepository.findById(id).map(server -> {
            server.setLastHeartbeat(LocalDateTime.now());
            serverRepository.save(server);
            return ResponseEntity.ok().<Void>build();
        }).orElse(ResponseEntity.notFound().build());
    }

    @GetMapping("/list")
    public ResponseEntity<List<GameServer>> listServers() {
        // Only return Player-Hosted servers (Private/Custom), hide Dedicated ones
        return ResponseEntity.ok(serverRepository.findByStatus("ONLINE").stream()
                .filter(s -> !s.isDedicated())
                .toList());
    }

    @PostMapping("/unregister/{id}")
    public ResponseEntity<Void> unregisterServer(@PathVariable String id) {
        if (serverRepository.existsById(id)) {
            serverRepository.deleteById(id);
            return ResponseEntity.ok().build();
        }
        return ResponseEntity.notFound().build();
    }

    @GetMapping("/find")
    public ResponseEntity<GameServer> findServer() {
        // Matchmaking: Prioritize Dedicated Servers
        List<GameServer> servers = serverRepository.findByStatus("ONLINE");
        
        if (servers.isEmpty()) {
            return ResponseEntity.noContent().build();
        }
        
        // Find first dedicated server
        return servers.stream()
            .filter(GameServer::isDedicated)
            .findFirst()
            .map(ResponseEntity::ok)
            .orElse(ResponseEntity.ok(servers.get(0))); // Fallback to any server
    }
}
