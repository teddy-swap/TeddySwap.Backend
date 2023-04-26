FROM denoland/deno:latest
COPY . /app
WORKDIR /app/src
ENTRYPOINT ["deno", "task", "start", "--lock=deno.lock --lock-write"]