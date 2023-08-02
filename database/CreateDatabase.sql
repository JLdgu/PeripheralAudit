CREATE TABLE IF NOT EXISTS "Site" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Site" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS "Location" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Location" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "SiteId" INTEGER NOT NULL,
    "DeskCount" INTEGER NOT NULL,
    "MonitorSingleCount" INTEGER NOT NULL,
    "MonitorDualCount" INTEGER NOT NULL,
    "MonitorGradeBronzeCount" INTEGER NOT NULL,
    "MonitorGradeSilverCount" INTEGER NOT NULL,
    "MonitorGradeGoldCount" INTEGER NOT NULL,
    "DockCount" INTEGER NOT NULL,
    "DockPsuCount" INTEGER NOT NULL,
    "PcCount" INTEGER NOT NULL,
    "KeyboardCount" INTEGER NOT NULL,
    "MouseCount" INTEGER NOT NULL,
    "ChairCount" INTEGER NULL,
    "Note" TEXT NULL, LastUpdate TEXT NOT NULL,
    CONSTRAINT "FK_Location_Site_SiteId" FOREIGN KEY ("SiteId") REFERENCES "Site" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Dock" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Dock" PRIMARY KEY AUTOINCREMENT,
    "LocationId" INTEGER NOT NULL,
    "SerialNumber" TEXT NULL,
    "AssetTag" TEXT NULL,
    "Manufacturer" TEXT NOT NULL,
    "Model" TEXT NOT NULL,
    CONSTRAINT "FK_Dock_Location_LocationId" FOREIGN KEY ("LocationId") REFERENCES "Location" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Monitor" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Monitor" PRIMARY KEY AUTOINCREMENT,
    "Size" INTEGER NOT NULL,
    "LocationId" INTEGER NOT NULL,
    "SerialNumber" TEXT NULL,
    "AssetTag" TEXT NULL,
    "Manufacturer" TEXT NOT NULL,
    "Model" TEXT NOT NULL,
    CONSTRAINT "FK_Monitor_Location_LocationId" FOREIGN KEY ("LocationId") REFERENCES "Location" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Dock_LocationId" ON "Dock" ("LocationId");
CREATE INDEX "IX_Location_SiteId" ON "Location" ("SiteId");
CREATE INDEX "IX_Monitor_LocationId" ON "Monitor" ("LocationId");
