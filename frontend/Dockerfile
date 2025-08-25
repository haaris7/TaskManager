FROM node:20 AS builder
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy source and build
COPY . .
ENV REACT_APP_BASE_URL=http://localhost:5000
RUN npm run build

# Serve static build
RUN npm install -g serve
CMD ["serve", "-s", "build", "-l", "3000"]
