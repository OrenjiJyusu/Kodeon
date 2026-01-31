package com.kylestudio.kodeon.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
@EnableWebSecurity
public class SecurityConfig {

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        http
            .csrf(csrf -> csrf.disable()) // Disable CSRF for game server POSTs
            .authorizeHttpRequests(auth -> auth
                .requestMatchers("/server/**").permitAll() // Allow server registration
                .requestMatchers("/auth/**").permitAll() // Allow auth endpoints
                .anyRequest().authenticated()
            );
        return http.build();
    }
}
