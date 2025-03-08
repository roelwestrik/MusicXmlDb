FROM node:lts-alpine AS build-stage
RUN npm install -g http-server

WORKDIR /app
COPY ./MusicXmlDb.Client/package*.json ./
RUN npm install

COPY ./MusicXmlDb.Client* .
RUN npm run build

EXPOSE 8080
CMD [ "http-server", "dist", "--host", "0.0.0.0"]