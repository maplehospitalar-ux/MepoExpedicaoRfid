-- Register device r3-desktop-02 in rfid_devices

INSERT INTO rfid_devices (device_id, device_name, location, is_active, created_by)
SELECT 
  'r3-desktop-02', 
  'Desktop C# - Expedição', 
  'Expedição', 
  true,
  id
FROM auth.users 
WHERE email = 'admin@rfid-system.local'
ON CONFLICT (device_id) DO NOTHING;
