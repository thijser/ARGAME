TEMPLATE = subdirs

SUBDIRS = Main \
          Server \
          UnitTests

Main.depends       = Server
UnitTests.depends  = Server
