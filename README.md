# GameSystem

## Deploy service to Raspberry PI

```
nuke --target StopService --service-name %SERVICE_NAME% --sshHost %HOST% --sshUserName %USER_NAME% --sshPassword %PASSWORD%
nuke --target DeployDotNet --targetProject %PROJECT_NAME% --targetRuntime linux-arm --selfContained true --localPiHome %LOCAL_PI_DIRECTORY% \
nuke --target StartService --service-name %SERVICE_NAME% --sshHost %HOST% --sshUserName %USER_NAME% --sshPassword %PASSWORD%
```

## Start service via systemd manually

1) Copy %PROJECT_NAME%/%SERVICE_NAME%.service to /etc/systemd/system
2) `sudo systemctl daemon-reload`
3) `sudo systemctl enable %SERVICE_NAME%`
4) `sudo systemctl start %SERVICE_NAME%`
