TEMPLATE = subdirs

SUBDIRS = Main \
          QHttpServer \
          Server \
          UnitTests

Server.depends     = QHttpServer
Main.depends       = Server
UnitTests.depends  = Server
