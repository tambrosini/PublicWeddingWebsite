#!/bin/bash

# Build script for containerized wedding website
set -e

echo "=== Building Wedding Website in Container ==="

# Build with Docker
echo "Building Docker image..."
docker build -t wedding-website:latest .

echo "=== Build completed successfully! ==="
echo ""
echo "To run the application:"
echo "  docker run -p 8080:80 wedding-website:latest"
echo ""
echo "Or use docker-compose:"
echo "  docker-compose up"
echo ""
echo "The application will be available at http://localhost:8080"
