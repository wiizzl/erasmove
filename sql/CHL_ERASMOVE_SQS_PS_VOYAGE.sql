/* =========================================================================
   PROCEDURE : PSS_VOYAGE
   DESCRIPTION : SELECTIONNE LES VOYAGES (TOUS OU PAR UTILISATEUR)
   ========================================================================= */
CREATE PROCEDURE PSS_VOYAGE
    @VOY_ID INT = NULL,
    @UTI_ID INT = NULL
AS
BEGIN
    SELECT V.VOY_ID, V.VOY_LIBELLE, V.VOY_DATE_CREATION, V.UTI_ID,
           U.UTI_NOM, U.UTI_PRENOM
    FROM VOYAGE V
    INNER JOIN UTILISATEUR U ON V.UTI_ID = U.UTI_ID
    WHERE (@VOY_ID IS NULL OR V.VOY_ID = @VOY_ID)
      AND (@UTI_ID IS NULL OR V.UTI_ID = @UTI_ID)
    ORDER BY V.VOY_DATE_CREATION DESC;
END;

/* =========================================================================
   PROCEDURE : PSI_VOYAGE
   DESCRIPTION : CREE UN NOUVEAU DOSSIER DE VOYAGE POUR UN ETUDIANT
   ========================================================================= */
CREATE PROCEDURE PSI_VOYAGE
    @VOY_LIBELLE VARCHAR(100),
    @UTI_ID INT,
    @NEW_ID INT OUTPUT
AS
BEGIN
    INSERT INTO VOYAGE (VOY_LIBELLE, UTI_ID, VOY_DATE_CREATION)
    VALUES (@VOY_LIBELLE, @UTI_ID, GETDATE());

    SET @NEW_ID = SCOPE_IDENTITY();
END;

/* =========================================================================
   PROCEDURE : PSD_VOYAGE
   DESCRIPTION : SUPPRIME UN VOYAGE ET SES ETAPES ASSOCIEES
   ========================================================================= */
CREATE PROCEDURE PSD_VOYAGE
    @VOY_ID INT
AS
BEGIN
    -- On supprime d'abord les étapes pour respecter l'intégrité référentielle
    DELETE FROM VOYAGE_ETAPE WHERE VOY_ID = @VOY_ID;
    DELETE FROM VOYAGE WHERE VOY_ID = @VOY_ID;
END;

/* =========================================================================
   PROCEDURE : PSI_VOYAGE_ETAPE
   DESCRIPTION : AJOUTE UN SEGMENT DU CATALOGUE A UN VOYAGE SPECIFIQUE
   ========================================================================= */
CREATE PROCEDURE PSI_VOYAGE_ETAPE
    @VOY_ID INT,
    @TRJ_ID INT,
    @VET_ORDRE INT
AS
BEGIN
    INSERT INTO VOYAGE_ETAPE (VOY_ID, TRJ_ID, VET_ORDRE)
    VALUES (@VOY_ID, @TRJ_ID, @VET_ORDRE);
END;

/* =========================================================================
   PROCEDURE : PSS_VOYAGE_ITINERARY
   DESCRIPTION : SELECTIONNE TOUTES LES ETAPES DETAILLEES D'UN VOYAGE
   ========================================================================= */
CREATE OR ALTER PROCEDURE PSS_VOYAGE_ITINERARY
    @VOY_ID INT
AS
BEGIN
    SELECT VE.VET_ORDRE, 
           LD.LIE_VILLE AS VILLE_DEPART, LD.LIE_LATITUDE AS LAT_DEPART, LD.LIE_LONGITUDE AS LON_DEPART,
           LA.LIE_VILLE AS VILLE_ARRIVEE, LA.LIE_LATITUDE AS LAT_ARRIVEE, LA.LIE_LONGITUDE AS LON_ARRIVEE,
           T.TRA_COMPAGNIE, TYP.TYP_LIBELLE
    FROM VOYAGE_ETAPE VE
    INNER JOIN TRAJET TRJ ON VE.TRJ_ID = TRJ.TRJ_ID
    INNER JOIN LIEU LD ON TRJ.LIE_ID_DEPART = LD.LIE_ID
    INNER JOIN LIEU LA ON TRJ.LIE_ID_ARRIVEE = LA.LIE_ID
    INNER JOIN TRANSPORT T ON TRJ.TRA_ID = T.TRA_ID
    INNER JOIN TYPE_TRANSPORT TYP ON T.TYP_ID = TYP.TYP_ID
    WHERE VE.VOY_ID = @VOY_ID
    ORDER BY VE.VET_ORDRE ASC;
END;