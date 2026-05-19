-- SQLite Manufacturing Tables Creation Script
-- Products, Bill of Materials, and Inventory

-- ============================================
-- Products Table
-- ============================================
CREATE TABLE IF NOT EXISTS Products (
    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_name TEXT NOT NULL,
    product_code TEXT NOT NULL UNIQUE,
    product_type TEXT NOT NULL CHECK(product_type IN ('Finished', 'Component')),
    unit_of_measure TEXT NOT NULL,
    cost_per_unit REAL NOT NULL DEFAULT 0.00,
    created_at TEXT NOT NULL DEFAULT (datetime('now')),
    CONSTRAINT chk_cost_positive CHECK(cost_per_unit >= 0)
);

-- Index for faster lookups by product code
CREATE INDEX IF NOT EXISTS idx_products_code ON Products(product_code);

-- Index for filtering by product type
CREATE INDEX IF NOT EXISTS idx_products_type ON Products(product_type);

-- ============================================
-- Bill_of_Materials Table
-- ============================================
CREATE TABLE IF NOT EXISTS Bill_of_Materials (
    bom_id INTEGER PRIMARY KEY AUTOINCREMENT,
    parent_product_id INTEGER NOT NULL,
    component_id INTEGER NOT NULL,
    quantity REAL NOT NULL,
    scrap_factor REAL NOT NULL DEFAULT 0.00,
    effective_date TEXT NOT NULL DEFAULT (date('now')),
    expiry_date TEXT,
    FOREIGN KEY (parent_product_id) REFERENCES Products(product_id) ON DELETE CASCADE,
    FOREIGN KEY (component_id) REFERENCES Products(product_id) ON DELETE CASCADE,
    CONSTRAINT chk_quantity_positive CHECK(quantity > 0),
    CONSTRAINT chk_scrap_factor_range CHECK(scrap_factor >= 0 AND scrap_factor <= 100),
    CONSTRAINT chk_different_products CHECK(parent_product_id != component_id),
    CONSTRAINT chk_date_order CHECK(expiry_date IS NULL OR expiry_date >= effective_date)
);

-- Index for finding all components of a finished product
CREATE INDEX IF NOT EXISTS idx_bom_parent ON Bill_of_Materials(parent_product_id);

-- Index for finding where a component is used
CREATE INDEX IF NOT EXISTS idx_bom_component ON Bill_of_Materials(component_id);

-- Index for date-based queries
CREATE INDEX IF NOT EXISTS idx_bom_dates ON Bill_of_Materials(effective_date, expiry_date);

-- Composite unique index to prevent duplicate BOM entries for the same dates
CREATE UNIQUE INDEX IF NOT EXISTS idx_bom_unique ON Bill_of_Materials(parent_product_id, component_id, effective_date);

-- ============================================
-- Inventory Table
-- ============================================
CREATE TABLE IF NOT EXISTS Inventory (
    inventory_id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id INTEGER NOT NULL,
    location TEXT NOT NULL,
    quantity_on_hand REAL NOT NULL DEFAULT 0.00,
    last_updated TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (product_id) REFERENCES Products(product_id) ON DELETE CASCADE,
    CONSTRAINT chk_quantity_non_negative CHECK(quantity_on_hand >= 0)
);

-- Index for finding inventory by product
CREATE INDEX IF NOT EXISTS idx_inventory_product ON Inventory(product_id);

-- Index for finding inventory by location
CREATE INDEX IF NOT EXISTS idx_inventory_location ON Inventory(location);

-- Unique constraint: one inventory record per product per location
CREATE UNIQUE INDEX IF NOT EXISTS idx_inventory_unique ON Inventory(product_id, location);

-- ============================================
-- Sample Data (Optional - for testing)
-- ============================================

-- INSERT INTO Products (product_name, product_code, product_type, unit_of_measure, cost_per_unit) VALUES
-- ('Finished Widget', 'FW-001', 'Finished', 'pcs', 25.00),
-- ('Component A', 'CA-001', 'Component', 'pcs', 5.00),
-- ('Component B', 'CB-001', 'Component', 'pcs', 3.50),
-- ('Raw Material C', 'RM-001', 'Component', 'kg', 1.25);

-- INSERT INTO Bill_of_Materials (parent_product_id, component_id, quantity, scrap_factor, effective_date) VALUES
-- (1, 2, 2.000, 5.00, date('now')),
-- (1, 3, 1.000, 2.50, date('now')),
-- (1, 4, 0.500, 1.00, date('now'));

-- INSERT INTO Inventory (product_id, location, quantity_on_hand) VALUES
-- (1, 'Warehouse A', 100.000),
-- (2, 'Warehouse A', 500.000),
-- (3, 'Warehouse B', 300.000),
-- (4, 'Warehouse A', 1000.000);
