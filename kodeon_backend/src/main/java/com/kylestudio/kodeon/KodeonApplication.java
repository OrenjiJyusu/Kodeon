package com.kylestudio.kodeon;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class KodeonApplication {

	public static void main(String[] args) {
		SpringApplication.run(KodeonApplication.class, args);
	}

}
