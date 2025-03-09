FROM node:lts-alpine AS build-stage
RUN npm install -g http-server

WORKDIR /app
COPY ./package*.json .
RUN npm install

COPY . .
RUN npm run build

EXPOSE 8080
CMD [ "http-server", "dist", "--host", "0.0.0.0"]