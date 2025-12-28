package main

import (
	"errors"
	"net/http"
	"os"
	"time"

	"github.com/golang-jwt/jwt/v5"
)

const (
	cookieAuthToken = cookiePrefix + "auth-token"
)

type AuthClaims struct {
	UserID   string `json:"userId"`
	Username string `json:"username"`
	Email    string `json:"email"`
	jwt.RegisteredClaims
}

func getJWTSecret() string {
	secret := os.Getenv("JWT_SECRET_KEY")
	if secret == "" {
		return "your-super-secret-jwt-key-change-this-in-production-must-be-at-least-32-characters-long"
	}
	return secret
}

func validateJWTToken(tokenString string) (string, error) {
	token, err := jwt.ParseWithClaims(tokenString, &AuthClaims{}, func(token *jwt.Token) (interface{}, error) {
		return []byte(getJWTSecret()), nil
	})

	if err != nil {
		return "", err
	}

	if claims, ok := token.Claims.(*AuthClaims); ok && token.Valid {
		if claims.ExpiresAt != nil && claims.ExpiresAt.Time.Before(time.Now()) {
			return "", errors.New("token expired")
		}
		return claims.UserID, nil
	}

	return "", errors.New("invalid token")
}

func userIDFromJWT(r *http.Request) string {
	cookie, err := r.Cookie(cookieAuthToken)
	if err == nil {
		userID, err := validateJWTToken(cookie.Value)
		if err == nil {
			return userID
		}
	}

	authHeader := r.Header.Get("Authorization")
	if len(authHeader) > 7 && authHeader[:7] == "Bearer " {
		tokenString := authHeader[7:]
		userID, err := validateJWTToken(tokenString)
		if err == nil {
			return userID
		}
	}

	return ""
}

func getUserID(r *http.Request) string {
	if userID := userIDFromJWT(r); userID != "" {
		return userID
	}

	return sessionID(r)
}

func getAuthClaimsFromJWT(r *http.Request) (*AuthClaims, error) {
	cookie, err := r.Cookie(cookieAuthToken)
	if err == nil {
		token, err := jwt.ParseWithClaims(cookie.Value, &AuthClaims{}, func(token *jwt.Token) (interface{}, error) {
			return []byte(getJWTSecret()), nil
		})
		if err == nil {
			if claims, ok := token.Claims.(*AuthClaims); ok && token.Valid {
				if claims.ExpiresAt == nil || claims.ExpiresAt.Time.After(time.Now()) {
					return claims, nil
				}
			}
		}
	}

	authHeader := r.Header.Get("Authorization")
	if len(authHeader) > 7 && authHeader[:7] == "Bearer " {
		tokenString := authHeader[7:]
		token, err := jwt.ParseWithClaims(tokenString, &AuthClaims{}, func(token *jwt.Token) (interface{}, error) {
			return []byte(getJWTSecret()), nil
		})
		if err == nil {
			if claims, ok := token.Claims.(*AuthClaims); ok && token.Valid {
				if claims.ExpiresAt == nil || claims.ExpiresAt.Time.After(time.Now()) {
					return claims, nil
				}
			}
		}
	}

	return nil, errors.New("no valid JWT token found")
}

func getUsernameFromJWT(r *http.Request) string {
	claims, err := getAuthClaimsFromJWT(r)
	if err == nil {
		return claims.Username
	}
	return ""
}

func getUserEmailFromJWT(r *http.Request) string {
	claims, err := getAuthClaimsFromJWT(r)
	if err == nil {
		return claims.Email
	}
	return ""
}

func isAuthenticated(r *http.Request) bool {
	_, err := getAuthClaimsFromJWT(r)
	return err == nil
}
