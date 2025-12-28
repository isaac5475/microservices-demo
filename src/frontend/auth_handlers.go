package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"os"
	"time"
)

const (
	cookieMaxAgeAuth = 60 * 60 * 12
)

type LoginRequest struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

type RegisterRequest struct {
	Username string `json:"username"`
	Email    string `json:"email"`
	Password string `json:"password"`
}

type AuthResponse struct {
	AccessToken  string `json:"accessToken"`
	RefreshToken string `json:"refreshToken"`
	UserID       string `json:"userId"`
	Username     string `json:"username"`
	Email        string `json:"email"`
}

func getAuthServiceURL() string {
	url := os.Getenv("AUTH_SERVICE_ADDR")
	if url == "" {
		return "http://authservice:5000"
	}
	return url
}

func (fe *frontendServer) loginHandler(w http.ResponseWriter, r *http.Request) {
	log := r.Context().Value(ctxKeyLog{}).(logrus.FieldLogger)

	var req LoginRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Invalid request", http.StatusBadRequest)
		return
	}

	jsonData, _ := json.Marshal(req)
	resp, err := http.Post(getAuthServiceURL()+"/api/users/login", "application/json", bytes.NewBuffer(jsonData))
	if err != nil {
		log.Errorf("Failed to call auth service: %v", err)
		http.Error(w, "Authentication service unavailable", http.StatusServiceUnavailable)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		body, _ := io.ReadAll(resp.Body)
		log.Warnf("Login failed: %s", string(body))
		http.Error(w, "Invalid credentials", http.StatusUnauthorized)
		return
	}

	var authResp AuthResponse
	if err := json.NewDecoder(resp.Body).Decode(&authResp); err != nil {
		log.Errorf("Failed to parse auth response: %v", err)
		http.Error(w, "Internal error", http.StatusInternalServerError)
		return
	}

	http.SetCookie(w, &http.Cookie{
		Name:     cookieAuthToken,
		Value:    authResp.AccessToken,
		MaxAge:   cookieMaxAgeAuth,
		Path:     "/",
		HttpOnly: true,
		SameSite: http.SameSiteLaxMode,
	})

	log.Infof("User logged in: %s", authResp.Username)
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(authResp)
}

func (fe *frontendServer) registerHandler(w http.ResponseWriter, r *http.Request) {
	log := r.Context().Value(ctxKeyLog{}).(logrus.FieldLogger)

	var req RegisterRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Invalid request", http.StatusBadRequest)
		return
	}

	jsonData, _ := json.Marshal(req)
	resp, err := http.Post(getAuthServiceURL()+"/api/users/register", "application/json", bytes.NewBuffer(jsonData))
	if err != nil {
		log.Errorf("Failed to call auth service: %v", err)
		http.Error(w, "Authentication service unavailable", http.StatusServiceUnavailable)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		body, _ := io.ReadAll(resp.Body)
		log.Warnf("Registration failed: %s", string(body))
		http.Error(w, string(body), resp.StatusCode)
		return
	}

	var authResp AuthResponse
	if err := json.NewDecoder(resp.Body).Decode(&authResp); err != nil {
		log.Errorf("Failed to parse auth response: %v", err)
		http.Error(w, "Internal error", http.StatusInternalServerError)
		return
	}

	http.SetCookie(w, &http.Cookie{
		Name:     cookieAuthToken,
		Value:    authResp.AccessToken,
		MaxAge:   cookieMaxAgeAuth,
		Path:     "/",
		HttpOnly: true,
		SameSite: http.SameSiteLaxMode,
	})

	log.Infof("User registered: %s", authResp.Username)
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(authResp)
}

func (fe *frontendServer) logoutHandler(w http.ResponseWriter, r *http.Request) {
	http.SetCookie(w, &http.Cookie{
		Name:     cookieAuthToken,
		Value:    "",
		MaxAge:   -1,
		Path:     "/",
		HttpOnly: true,
	})

	http.Redirect(w, r, "/", http.StatusSeeOther)
}
