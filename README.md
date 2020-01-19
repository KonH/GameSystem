# GameSystem

## Deploy service to Raspberry PI

```
nuke --target DeployDotNet --targetProject %PROJECT_NAME% --targetRuntime linux-arm --selfContained true --localPiHome %LOCAL_PI_DIRECTORY%
```

## Start service via systemd

1) Copy %PROJECT_NAME%/%SERVICE_NAME%.service to /etc/systemd/system
2) `systemctl daemon-reload`
3) `systemctl enable %SERVICE_NAME%`
4) `systemctl start %SERVICE_NAME%`
