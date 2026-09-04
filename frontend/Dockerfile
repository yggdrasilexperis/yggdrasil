# Build stage 1: Install dependencies
FROM node:22-alpine AS deps

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Build stage 2: Build the application
FROM node:22-alpine AS build

# Set working directory
WORKDIR /app

# Copy dependencies from previous stage
COPY --from=deps /app/node_modules ./node_modules

# Copy application files
COPY . ./

# Set environment variables
ARG VITE_API_BASE_URL

# Export environment variables
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}

# Build the application
RUN npm run build

# Runtime stage: Serve the application
FROM nginx:alpine

# Copy the Nginx configuration file
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the built application from the build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Expose the HTTP port
EXPOSE 80

# Expose the HTTPS port
# EXPOSE 443