@echo off

md %4\platforms

if "%1"=="debug" (
    echo Deploying debug libraries
    echo =========================
    xcopy %3\bin\Qt5Cored.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Guid.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Networkd.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Widgetsd.dll %4 /D /I /Y
    xcopy %2\bin\opencv_core300d.dll %4 /D /I /Y
    xcopy %2\bin\opencv_imgproc300d.dll %4 /D /I /Y
    xcopy %2\bin\opencv_highgui300d.dll %4 /D /I /Y
    xcopy %2\bin\opencv_video300d.dll %4 /D /I /Y
    xcopy %2\bin\opencv_videoio300d.dll %4 /D /I /Y
    xcopy %2\bin\opencv_imgcodecs300d.dll %4 /D /I /Y
    xcopy %3\plugins\platforms\qwindowsd.dll %4\platforms /D /I /Y
) 

if "%1"=="release" (
    echo Deploying release libraries
    echo ===========================
    xcopy %3\bin\Qt5Core.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Gui.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Network.dll %4 /D /I /Y
    xcopy %3\bin\Qt5Widgets.dll %4 /D /I /Y
    xcopy %2\bin\opencv_core300.dll %4 /D /I /Y
    xcopy %2\bin\opencv_imgproc300.dll %4 /D /I /Y
    xcopy %2\bin\opencv_highgui300.dll %4 /D /I /Y
    xcopy %2\bin\opencv_video300.dll %4 /D /I /Y
    xcopy %2\bin\opencv_videoio300.dll %4 /D /I /Y
    xcopy %2\bin\opencv_imgcodecs300.dll %4 /D /I /Y
    xcopy %3\plugins\platforms\qwindows.dll %4\platforms /D /I /Y
)

REM - ICU doesn't have a debug version
xcopy %3\bin\icu*.dll %4 /D /I /Y
xcopy %4\..\MirrorServer\markers %4\markers /D /I /Y
@echo on