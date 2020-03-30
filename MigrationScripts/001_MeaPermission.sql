DO LANGUAGE plpgsql $tran$
BEGIN

DO $$
BEGIN
    IF NOT EXISTS(
        SELECT schema_name
          FROM information_schema.schemata
          WHERE schema_name = 'public'
      )
    THEN
      EXECUTE 'CREATE SCHEMA public';
    END IF;

END
$$;

DROP TABLE IF EXISTS public.mt_doc_meapermission CASCADE;
CREATE TABLE public.mt_doc_meapermission (
    id                  uuid CONSTRAINT pk_mt_doc_meapermission PRIMARY KEY,
    data                jsonb NOT NULL,
    mt_last_modified    timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version          uuid NOT NULL default(md5(random()::text || clock_timestamp()::text)::uuid),
    mt_dotnet_type      varchar ,
    mt_deleted          boolean DEFAULT FALSE,
    mt_deleted_at       timestamp with time zone NULL
);
COMMENT ON TABLE public.mt_doc_meapermission IS 'origin:Marten.IDocumentStore, Marten, Version=3.10.0.0, Culture=neutral, PublicKeyToken=null';

CREATE INDEX mt_doc_meapermission_idx_mt_deleted ON public.mt_doc_meapermission ("mt_deleted");

CREATE OR REPLACE FUNCTION public.mt_upsert_meapermission(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_meapermission ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT ON CONSTRAINT pk_mt_doc_meapermission
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where public.mt_doc_meapermission.mt_version = current_version;

  SELECT mt_version FROM public.mt_doc_meapermission into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_insert_meapermission(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
BEGIN
INSERT INTO public.mt_doc_meapermission ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_update_meapermission(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_meapermission SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where public.mt_doc_meapermission.mt_version = current_version and id = docId;

  SELECT mt_version FROM public.mt_doc_meapermission into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;


CREATE OR REPLACE FUNCTION public.mt_overwrite_meapermission(current_version uuid, doc JSONB, docDotNetType varchar, docId uuid, docVersion uuid) RETURNS UUID LANGUAGE plpgsql SECURITY INVOKER AS $function$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_meapermission ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT ON CONSTRAINT pk_mt_doc_meapermission
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_meapermission into final_version WHERE id = docId ;
  RETURN final_version;
END;
$function$;



END;
$tran$;
